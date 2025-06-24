using Application.Queries;
using Core.Entities;
using Core.Repositories.Query;
using Dapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Google.Api.Gax.ResourceNames;
using Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
                    var query = @"
                -- First delete from ApplicationRolePermission
                WITH RecursivePermissionCTE AS
                (   
                    SELECT PermissionID, ParentID
                    FROM ApplicationPermission
                    WHERE PermissionID = @permissionid
                    UNION ALL 
                    SELECT ap.PermissionID, ap.ParentID
                    FROM ApplicationPermission ap
                    INNER JOIN RecursivePermissionCTE r
                        ON ap.ParentID = r.PermissionID
                )
                DELETE FROM ApplicationRolePermission
                WHERE PermissionID IN (SELECT PermissionID FROM RecursivePermissionCTE);

                -- Then delete from ApplicationPermission
                WITH RecursivePermissionCTE AS
                (   
                    SELECT PermissionID, ParentID
                    FROM ApplicationPermission
                    WHERE PermissionID = @permissionid
                    UNION ALL 
                    SELECT ap.PermissionID, ap.ParentID
                    FROM ApplicationPermission ap
                    INNER JOIN RecursivePermissionCTE r
                        ON ap.ParentID = r.PermissionID
                )
                DELETE FROM ApplicationPermission
                WHERE PermissionID IN (SELECT PermissionID FROM RecursivePermissionCTE);";

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
              
                var query = "SELECT * FROM ApplicationPermission WHERE ParentID=60400 ORDER BY PermissionOrder";

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
                    //string nextPermissionLevel = "A01";

                    //var allPermissions = await GetAllAsync();
                    //var nextpermission = allPermissions.OrderByDescending(p => p.PermissionOrder).FirstOrDefault();

                    if (checkLink != null && checkLink.PermissionID > 0)
                    {
                        permissionid = (long)checkLink.PermissionID + 1;

                        //if (nextpermission != null)
                        //{
                        //    if (!string.IsNullOrEmpty(nextpermission.PermissionOrder))
                        //    {
                        //        int currentLevelNumber = int.Parse(nextpermission.PermissionOrder.Substring(1));
                        //        nextPermissionLevel = "A" + (currentLevelNumber + 1).ToString();
                        //    }
                        //}

                    }

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("PermissionID", permissionid);                        
                        parameters.Add("PermissionURL", applicationPermission.PermissionURL, DbType.String);
                        parameters.Add("PermissionName", applicationPermission.PermissionName, DbType.String);
                        //parameters.Add("PermissionLevel", nextPermissionLevel, DbType.String);
                        parameters.Add("PermissionLevel", applicationPermission.PermissionOrder);

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
                        parameters.Add("PermissionOrder", applicationPermission.PermissionOrder, DbType.String);

                        var query = "UPDATE ApplicationPermission SET PermissionURL = @PermissionURL ,PermissionName = @PermissionName,PermissionOrder = @PermissionOrder WHERE PermissionID = @PermissionID";
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

        async Task<long> IApplicationPermissionListQueryRepository.UpdateOrder(ApplicationPermission applicationPermission)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("Name", applicationPermission.Name, DbType.String);
                        parameters.Add("PermissionURL", applicationPermission.PermissionURL, DbType.String);
                        parameters.Add("PermissionName", applicationPermission.PermissionName, DbType.String);
                        parameters.Add("PermissionOrder", applicationPermission.PermissionOrder, DbType.String);

                        var query = "UPDATE ApplicationPermission SET PermissionOrder = @PermissionOrder WHERE PermissionName = @PermissionName AND PermissionURL = @PermissionURL AND Name = @Name";
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

        public async Task<IReadOnlyList<ApplicationPermission>> GetAllListByParentIDAsync(string parentID)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("parentID", parentID);

                var query = "SELECT * FROM ApplicationPermission WHERE ParentID = @parentID";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ApplicationPermission>(query,parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<ApplicationPermission>> GetAllListByParentAsync()
        {
            try
            {
                var parameters = new DynamicParameters();

                var query = "SELECT * FROM ApplicationPermission WHERE ParentID IS NULL";

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

        public async Task<IReadOnlyList<ApplicationPermission>> GetAllListBySessionIDAsync(Guid? SessionID)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionID", SessionID);

                var query = "SELECT ParentID,PermissionID,PermissionName,PermissionURL,PermissionOrder FROM ApplicationPermission WHERE UniqueSessionID = @SessionID";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ApplicationPermission>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<long> InsertPermission(ApplicationPermission applicationPermission)
        {
            using (var connection = CreateConnection())
            {
                var checkLink = await GetApplicationPermissionTop1Async();
                long? permissionid = 0;
              

                if (checkLink != null && checkLink.PermissionID > 0)
                {
                    permissionid = (long)checkLink.PermissionID + 1;

                   
                }
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("PermissionID", permissionid);
                    parameters.Add("PermissionName", applicationPermission.PermissionName, DbType.String);
                    parameters.Add("ParentID", applicationPermission.ParentID);
                    parameters.Add("PermissionLevel", applicationPermission.PermissionLevel);
                    parameters.Add("PermissionOrder", applicationPermission.PermissionOrder);
                    parameters.Add("IsHeader", applicationPermission.IsHeader);
                    parameters.Add("PermissionURL", applicationPermission.PermissionURL);
                    parameters.Add("Component", applicationPermission.Component);
                    parameters.Add("Name", applicationPermission.Name);
                    parameters.Add("IsPermissionURL", applicationPermission.IsPermissionURL);

                    var query = @"INSERT INTO ApplicationPermission
                                 (PermissionID,PermissionName,ParentID,PermissionLevel,PermissionOrder,IsDisplay,IsHeader,IsNewPortal,IsCmsApp,IsMobile,IsPermissionURL,
                                  Component,Name,PermissionURL)
                                  VALUES (@PermissionID,@PermissionName,@ParentID,@PermissionLevel,@PermissionOrder,1,@IsHeader,1,1,0,@IsPermissionURL,@Component,@Name,@PermissionURL)";
                    var rowsAffected = await connection.ExecuteAsync(query, parameters);

                    return rowsAffected;
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }
            }
        }
    }
}


