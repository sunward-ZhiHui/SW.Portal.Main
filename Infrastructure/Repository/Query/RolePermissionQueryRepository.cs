using Core.Entities;
using Core.EntityModels;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastructure.Repository.Query
{
    public class RolePermissionQueryRepository : QueryRepository<ApplicationRole>, IRolePermissionQueryRepository
    {
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public RolePermissionQueryRepository(IConfiguration configuration, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
            : base(configuration)
        {
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }

        public async Task<IReadOnlyList<ApplicationRole>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM ApplicationRole";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ApplicationRole>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ApplicationRole> GetApplicationRoleOne(long? RoleId)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("RoleId", RoleId);
                        var query = "select t1.*,t2.UserName as AddedBy,t3.UserName as ModifiedBy,t4.CodeValue as StatusCode from ApplicationRole t1\r\n" +
                            "LEFT JOIN ApplicationUser t2 ON t1.AddedByUserID=t2.UserID\r\n" +
                            "LEFT JOIN ApplicationUser t3 ON t1.ModifiedByUserID=t3.UserID\r\n" +
                            "LEFT JOIN CodeMaster t4 ON t1.StatusCodeID=t4.CodeID\r\n WHERE t1.RoleId= @RoleId;";

                        var result = await connection.QuerySingleOrDefaultAsync<ApplicationRole>(query, parameters);
                        return result;
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
        public async Task<long> Insert(ApplicationRole rolepermission)
        {

            try
            {
                using (var connection = CreateConnection())
                {


                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("RoleName", rolepermission.RoleName, DbType.String);
                        parameters.Add("RoleDescription", rolepermission.RoleDescription);

                        parameters.Add("AddedByUserID", rolepermission.AddedByUserID);
                        parameters.Add("AddedDate", rolepermission.AddedDate);
                        parameters.Add("ModifiedByUserID", rolepermission.ModifiedByUserID);
                        parameters.Add("ModifiedDate", rolepermission.ModifiedDate);
                        parameters.Add("StatusCodeID", rolepermission.StatusCodeID);

                        var query = "INSERT INTO ApplicationRole(RoleName,RoleDescription,AddedByUserID,AddedDate,StatusCodeID,ModifiedDate,ModifiedByUserID) OUTPUT INSERTED.RoleID VALUES (@RoleName,@RoleDescription,@AddedByUserID,@AddedDate,@StatusCodeID,@ModifiedDate,@ModifiedByUserID)";

                        var plantData = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);

                        var guid = Guid.NewGuid();
                        var uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRole", "Add", null, rolepermission?.RoleName, plantData, guid, rolepermission?.AddedByUserID, DateTime.Now, false, "RoleName", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRole", "Add", rolepermission?.RoleName, rolepermission?.RoleName, plantData, guid, rolepermission?.AddedByUserID, DateTime.Now, false, "DisplayName", uid);
                        if (!string.IsNullOrEmpty(rolepermission?.RoleDescription))
                        {
                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRole", "Add", null, rolepermission?.RoleDescription, plantData, guid, rolepermission?.AddedByUserID, DateTime.Now, false, "RoleDescription", uid);

                        }
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRole", "Add", null, rolepermission?.StatusCodeID?.ToString(), plantData, guid, rolepermission?.AddedByUserID, DateTime.Now, false, "StatusCodeID", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRole", "Add", null, rolepermission?.StatusCode, plantData, guid, rolepermission?.AddedByUserID, DateTime.Now, false, "StatusCode", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRole", "Add", null, rolepermission?.AddedByUserID?.ToString(), plantData, guid, rolepermission?.AddedByUserID, DateTime.Now, false, "AddedByUserID", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRole", "Add", null, rolepermission?.AddedDate != null ? rolepermission.AddedDate.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, plantData, guid, rolepermission?.AddedByUserID, DateTime.Now, false, "AddedDate", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRole", "Add", null, rolepermission?.AddedBy?.ToString(), plantData, guid, rolepermission?.AddedByUserID, DateTime.Now, false, "AddedBy", uid);

                        return plantData;
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
        public async Task<long> Update(ApplicationRole rolepermission)
        {
            try
            {
                using (var connection = CreateConnection())
                {


                    try
                    {
                        var result = await GetApplicationRoleOne(rolepermission.RoleID);
                        var parameters = new DynamicParameters();
                        parameters.Add("RoleName", rolepermission.RoleName);
                        parameters.Add("RoleDescription", rolepermission.RoleDescription);
                        parameters.Add("AddedByUserID", rolepermission.AddedByUserID);
                        parameters.Add("AddedDate", rolepermission.AddedDate);
                        parameters.Add("ModifiedByUserID", rolepermission.ModifiedByUserID);
                        parameters.Add("ModifiedDate", rolepermission.ModifiedDate);
                        parameters.Add("StatusCodeID", rolepermission.StatusCodeID);
                        parameters.Add("RoleID", rolepermission.RoleID, DbType.Int64);

                        var query = " UPDATE ApplicationRole SET ModifiedDate=@ModifiedDate, StatusCodeID=@StatusCodeID,AddedByUserID=@AddedByUserID,AddedDate=@AddedDate,ModifiedByUserID=@ModifiedByUserID,RoleName = @RoleName,RoleDescription = @RoleDescription WHERE RoleID = @RoleID";

                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        bool isUpdate = false;

                        var guid = Guid.NewGuid();
                        var uid = Guid.NewGuid();
                        if (result != null)
                        {
                            if (rolepermission?.RoleName != result?.RoleName)
                            {
                                isUpdate = true;
                                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRole", "Update", rolepermission?.RoleName, rolepermission?.RoleName, rolepermission.RoleID, guid, rolepermission?.ModifiedByUserID, DateTime.Now, false, "RoleName", uid);
                            }
                            if (rolepermission?.RoleDescription != result?.RoleDescription)
                            {
                                uid = Guid.NewGuid(); isUpdate = true;
                                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRole", "Update", result?.RoleDescription, rolepermission?.RoleDescription, rolepermission.RoleID, guid, rolepermission?.ModifiedByUserID, DateTime.Now, false, "RoleDescription", uid);

                            }
                            if (rolepermission?.StatusCodeID != result?.StatusCodeID)
                            {
                                uid = Guid.NewGuid(); isUpdate = true;
                                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRole", "Update", result?.StatusCodeID?.ToString(), rolepermission?.StatusCodeID?.ToString(), rolepermission.RoleID, guid, rolepermission?.ModifiedByUserID, DateTime.Now, false, "StatusCodeID", uid);
                                uid = Guid.NewGuid();
                                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRole", "Update", result?.StatusCode, rolepermission?.StatusCode, rolepermission.RoleID, guid, rolepermission?.ModifiedByUserID, DateTime.Now, false, "StatusCode", uid);

                            }
                            if (isUpdate)
                            {
                                uid = Guid.NewGuid();
                                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRole", "Update", rolepermission?.RoleName, rolepermission?.RoleName, rolepermission.RoleID, guid, rolepermission?.ModifiedByUserID, DateTime.Now, false, "DisplayName", uid);
                                uid = Guid.NewGuid();
                                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRole", "Update", result?.ModifiedByUserID?.ToString(), rolepermission?.ModifiedByUserID?.ToString(), rolepermission.RoleID, guid, rolepermission?.ModifiedByUserID, DateTime.Now, false, "ModifiedByUserID", uid);
                                uid = Guid.NewGuid();
                                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRole", "Update", result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, rolepermission?.ModifiedDate != null ? rolepermission.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, rolepermission.RoleID, guid, rolepermission?.ModifiedByUserID, DateTime.Now, false, "ModifiedDate", uid);
                                uid = Guid.NewGuid();
                                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRole", "Update", result?.ModifiedBy?.ToString(), rolepermission?.ModifiedBy?.ToString(), rolepermission.RoleID, guid, rolepermission?.ModifiedByUserID, DateTime.Now, false, "ModifiedBy", uid);
                            }
                        }
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


        public async Task<long> Delete(long id, long? UserId)
        {
            try
            {
                using (var connection = CreateConnection())
                {



                    try
                    {
                        var rolepermission = await GetApplicationRoleOne(id);
                        var parameters = new DynamicParameters();
                        parameters.Add("RoleID", id);

                        var query = "DELETE  FROM ApplicationRole WHERE RoleID = @RoleID";


                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        var guid = Guid.NewGuid();
                        var uid = Guid.NewGuid();
                        if (rolepermission != null)
                        {

                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRole", "Delete", rolepermission?.RoleName, rolepermission?.RoleName, rolepermission.RoleID, guid, UserId, DateTime.Now, true, "RoleName", uid);
                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRole", "Delete", rolepermission?.RoleDescription, rolepermission?.RoleDescription, rolepermission.RoleID, guid, UserId, DateTime.Now, true, "RoleDescription", uid);
                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRole", "Delete", rolepermission?.StatusCodeID?.ToString(), rolepermission?.StatusCodeID?.ToString(), rolepermission.RoleID, guid, UserId, DateTime.Now, true, "StatusCodeID", uid);
                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRole", "Delete", rolepermission?.StatusCode, rolepermission?.StatusCode, rolepermission.RoleID, guid, UserId, DateTime.Now, true, "StatusCode", uid);


                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRole", "Delete", rolepermission?.RoleName, rolepermission?.RoleName, rolepermission.RoleID, guid, UserId, DateTime.Now, true, "DisplayName", uid);
                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRole", "Delete", rolepermission?.ModifiedByUserID?.ToString(), rolepermission?.ModifiedByUserID?.ToString(), rolepermission.RoleID, guid, UserId, DateTime.Now, true, "ModifiedByUserID", uid);
                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRole", "Delete", rolepermission?.ModifiedDate != null ? rolepermission.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, rolepermission?.ModifiedDate != null ? rolepermission.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, rolepermission.RoleID, guid, UserId, DateTime.Now, true, "ModifiedDate", uid);
                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRole", "Delete", rolepermission?.ModifiedBy?.ToString(), rolepermission?.ModifiedBy?.ToString(), rolepermission.RoleID, guid, UserId, DateTime.Now, true, "ModifiedBy", uid);
                        }


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

        public async Task<List<ApplicationPermission>> GetSelectedRolePermissionListAsync(long RoleId)
        {
            try
            {

                var parameters = new DynamicParameters();
                parameters.Add("RoleID", RoleId, DbType.Int64);

                //var query = @"SELECT ARP.PermissionID,ARP.PermissionName,ARP.ParentID FROM ApplicationPermission ARP 
                //            Left JOIN ApplicationRolePermission AP ON AP.PermissionID = ARP.PermissionID 
                //                  WHERE AP.RoleID = @RoleID and ARP.IsDisplay =1 and ARP.PermissionID > =60000";
                var query = @"SELECT
                                ap.PermissionID,
                                ap.PermissionName,
                                ap.ParentID,
                                CASE WHEN arp.RoleID IS NOT NULL THEN 'true' ELSE 'false' END AS Checked
                            FROM
                                ApplicationPermission ap
                            LEFT JOIN
                                ApplicationRolePermission arp ON ap.PermissionID = arp.PermissionID AND arp.RoleID = @RoleID
                            WHERE
                                ap.IsDisplay = 1
                                AND ap.IsNewPortal = 1
                                AND ap.IsCmsApp = 1
                                AND (ap.IsMobile IS NULL OR ap.IsMobile = 0)
                                AND ap.PermissionID > 60000
                            ORDER BY ap.PermissionOrder";
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
    }
}

