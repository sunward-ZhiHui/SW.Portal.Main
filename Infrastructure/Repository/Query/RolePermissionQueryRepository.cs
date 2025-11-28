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

                        List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
                        if (!string.IsNullOrEmpty(rolepermission?.RoleName))
                        {
                            auditList.Add(new HRMasterAuditTrail { CurrentValue = rolepermission?.RoleName, ColumnName = "RoleName" });
                            auditList.Add(new HRMasterAuditTrail { PreValue = rolepermission?.RoleName, CurrentValue = rolepermission?.RoleName, ColumnName = "DisplayName" });
                        }
                        if (!string.IsNullOrEmpty(rolepermission?.RoleDescription))
                        {
                            auditList.Add(new HRMasterAuditTrail { CurrentValue = rolepermission?.RoleDescription, ColumnName = "RoleDescription" });
                        }
                        auditList.Add(new HRMasterAuditTrail { CurrentValue = rolepermission?.StatusCodeID?.ToString(), ColumnName = "StatusCodeID" });
                        auditList.Add(new HRMasterAuditTrail { CurrentValue = rolepermission?.StatusCode, ColumnName = "StatusCode" });
                        auditList.Add(new HRMasterAuditTrail { CurrentValue = rolepermission?.AddedByUserID?.ToString(), ColumnName = "AddedByUserID" });
                        auditList.Add(new HRMasterAuditTrail { CurrentValue = rolepermission?.AddedDate != null ? rolepermission.AddedDate.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "AddedDate" });
                        auditList.Add(new HRMasterAuditTrail { CurrentValue = rolepermission?.AddedBy?.ToString(), ColumnName = "AddedBy" });
                        if (auditList.Count() > 0)
                        {
                            HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                            {
                                HRMasterAuditTrailItems = auditList,
                                Type = "ApplicationRole",
                                FormType = "Add",
                                HRMasterSetId = plantData,
                                AuditUserId = rolepermission?.AddedByUserID,
                            };
                            await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
                        }
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
                        if (result != null)
                        {
                            List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
                            if (rolepermission?.RoleName != result?.RoleName)
                            {
                                isUpdate = true;
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.RoleName, CurrentValue = rolepermission?.RoleName, ColumnName = "RoleName" });
                            }
                            if (rolepermission?.RoleDescription != result?.RoleDescription)
                            {
                                isUpdate = true;
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.RoleDescription, CurrentValue = rolepermission?.RoleDescription, ColumnName = "RoleDescription" });
                            }
                            if (rolepermission?.StatusCodeID != result?.StatusCodeID)
                            {
                                isUpdate = true;
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCodeID?.ToString(), CurrentValue = rolepermission?.StatusCodeID?.ToString(), ColumnName = "StatusCodeID" });
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCode, CurrentValue = rolepermission?.StatusCode, ColumnName = "StatusCode" });
                            }
                            if (isUpdate)
                            {
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.RoleName, CurrentValue = rolepermission?.RoleName, ColumnName = "DisplayName" });
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedByUserID?.ToString(), CurrentValue = rolepermission?.ModifiedByUserID?.ToString(), ColumnName = "ModifiedByUserID" });
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, CurrentValue = rolepermission?.ModifiedDate != null ? rolepermission.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "ModifiedDate" });
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedBy, CurrentValue = rolepermission?.ModifiedBy?.ToString(), ColumnName = "ModifiedBy" });
                            }
                            if (auditList.Count() > 0)
                            {
                                HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                                {
                                    HRMasterAuditTrailItems = auditList,
                                    Type = "ApplicationRole",
                                    FormType = "Update",
                                    HRMasterSetId = rolepermission?.RoleID,
                                    AuditUserId = rolepermission?.ModifiedByUserID,
                                };
                                await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
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
                        if (rolepermission != null)
                        {
                            List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();

                            auditList.Add(new HRMasterAuditTrail { PreValue = rolepermission?.RoleName, ColumnName = "RoleName" });
                            auditList.Add(new HRMasterAuditTrail { PreValue = rolepermission?.RoleName, CurrentValue = rolepermission?.RoleName, ColumnName = "DisplayName" });
                            auditList.Add(new HRMasterAuditTrail { PreValue = rolepermission?.RoleDescription, ColumnName = "RoleDescription" });
                            auditList.Add(new HRMasterAuditTrail { PreValue = rolepermission?.StatusCodeID?.ToString(), ColumnName = "StatusCodeID" });
                            auditList.Add(new HRMasterAuditTrail { PreValue = rolepermission?.StatusCode, ColumnName = "StatusCode" });
                            auditList.Add(new HRMasterAuditTrail { PreValue = rolepermission?.ModifiedByUserID?.ToString(), ColumnName = "ModifiedByUserID" });
                            auditList.Add(new HRMasterAuditTrail { PreValue = rolepermission?.ModifiedDate != null ? rolepermission.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "ModifiedDate" });
                            auditList.Add(new HRMasterAuditTrail { PreValue = rolepermission?.ModifiedBy?.ToString(), ColumnName = "ModifiedBy" });
                            if (auditList.Count() > 0)
                            {
                                HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                                {
                                    HRMasterAuditTrailItems = auditList,
                                    Type = "ApplicationRole",
                                    FormType = "Delete",
                                    HRMasterSetId = rolepermission?.RoleID,
                                    IsDeleted = true,
                                    AuditUserId = UserId,
                                };
                                await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
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

