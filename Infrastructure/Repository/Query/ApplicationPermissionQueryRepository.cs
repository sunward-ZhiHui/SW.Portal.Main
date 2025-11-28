using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using Dapper;
using DocumentFormat.OpenXml.Spreadsheet;
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
    public class ApplicationPermissionQueryRepository : QueryRepository<ApplicationPermission>, IApplicationPermissionQueryRepository
    {
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public ApplicationPermissionQueryRepository(IConfiguration configuration, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
            : base(configuration)
        {
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }

        public async Task<IReadOnlyList<ApplicationPermission>> GetAllAsync()
        {
            try
            {
                var query = "Select  * from view_UserPermission where IsDisplay=1 and IsNewPortal =1 and IsCmsApp =1  and (IsMobile is null or IsMobile=0) ORDER BY PermissionOrder";
                // var query = @"Select* from view_UserPermission where  IsDisplay = 1 and IsNewPortal = 1 and IsCmsApp = 1  and(IsMobile is null or IsMobile = 0) ORDER BY PermissionOrder";
                //  var query = @"select * from applicationpermission where IsDisplay = 1 and PermissionID > =60000";
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
        public async Task<IReadOnlyList<ApplicationRolePermission>> GetApplicationRolePermissionOne(long? RoleId)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("RoleId", RoleId);
                        var query = "select t1.*,t2.PermissionName from ApplicationRolePermission t1\r\nLEFT JOIN ApplicationPermission t2 ON t1.PermissionID=t2.PermissionID WHERE t1.RoleId= @RoleId AND t1.PermissionID>60000 and t2.IsDisplay=1 AND t2.IsNewPortal =1 and t2.IsCmsApp =1  and (t2.IsMobile is null or t2.IsMobile=0);";

                        return (await connection.QueryAsync<ApplicationRolePermission>(query, parameters)).ToList();
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
        public async Task<IReadOnlyList<ApplicationPermission>> GetAllApplicationPermissionAsync()
        {
            try
            {
                var query = "Select  t2.* from ApplicationPermission t2 where t2.PermissionID>60000 and  t2.IsDisplay=1 AND t2.IsNewPortal =1 and t2.IsCmsApp =1  and (t2.IsMobile is null or t2.IsMobile=0)  ORDER BY t2.PermissionID asc\r\n";
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
        public async Task<long> Insert(ApplicationRolePermission applicationrolepermission)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var resultAlls = await GetAllApplicationPermissionAsync();
                        var results = resultAlls.Any() ? resultAlls?.ToList() : new List<ApplicationPermission>();
                        var resultData = await GetApplicationRolePermissionOne(applicationrolepermission.RoleID);
                        var parameters = new DynamicParameters();
                        parameters.Add("RoleID", applicationrolepermission.RoleID);
                        parameters.Add("PermissionIDs", applicationrolepermission.PermissionIDs);
                        var result = connection.QueryFirstOrDefault<long>("sp_Ins_RolePermission", parameters, commandType: CommandType.StoredProcedure);
                        List<long> oldlist = new List<long>();
                        if (resultData != null && resultData.Count > 0)
                        {
                            oldlist = resultData.ToList().Select(s => s.PermissionID).ToList();
                        }
                        var newList = applicationrolepermission?.PermissionIDs?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt64(x)).ToList() ?? new List<long>();
                        oldlist = oldlist != null ? oldlist : new List<long>();
                        var added = newList.Except(oldlist).ToList();     // items in new but not old
                        var removed = oldlist.Except(newList).ToList();   // items in old but not new
                        string oldValues = string.Empty; string newValues = string.Empty;
                        var guid = Guid.NewGuid();
                        bool isUpdate = false;
                        List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
                        if (added.Any() || removed.Any())
                        {
                            if (added.Any())
                            {
                                foreach (var item in added)
                                {
                                    isUpdate = true;
                                    auditList.Add(new HRMasterAuditTrail { CurrentValue = item.ToString(), ColumnName = "PermissionID", FormType = "Add" });
                                    var names = results?.FirstOrDefault(f => f.PermissionID == item)?.PermissionName;
                                    auditList.Add(new HRMasterAuditTrail { CurrentValue = names, ColumnName = "PermissionName", FormType = "Add" });
                                }
                            }
                            if (removed.Any())
                            {
                                foreach (var item in removed)
                                {
                                    isUpdate = true;
                                    auditList.Add(new HRMasterAuditTrail { PreValue = item.ToString(), ColumnName = "PermissionID", FormType = "Removed" });
                                    var names = results?.FirstOrDefault(f => f.PermissionID == item)?.PermissionName;
                                    auditList.Add(new HRMasterAuditTrail { PreValue = names, ColumnName = "PermissionName", FormType = "Removed" });
                                }
                            }
                        }
                        if (isUpdate)
                        {
                            auditList.Add(new HRMasterAuditTrail { PreValue = applicationrolepermission?.RolePermissionName, CurrentValue = applicationrolepermission?.RolePermissionName, ColumnName = "DisplayName", FormType = "Add/Update" });
                            auditList.Add(new HRMasterAuditTrail { PreValue = applicationrolepermission?.AddedByUserID?.ToString(), CurrentValue = applicationrolepermission?.AddedByUserID?.ToString(), ColumnName = "AddedByUserID", FormType = "Add/Update" });
                            auditList.Add(new HRMasterAuditTrail { PreValue = applicationrolepermission?.AddedDate != null ? applicationrolepermission?.AddedDate.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, CurrentValue = applicationrolepermission?.AddedDate != null ? applicationrolepermission?.AddedDate.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "AddedDate", FormType = "Add/Update" });
                            auditList.Add(new HRMasterAuditTrail { PreValue = applicationrolepermission?.AddedBy?.ToString(), CurrentValue = applicationrolepermission?.AddedBy?.ToString(), ColumnName = "AddedBy", FormType = "Add/Update" });
                        }
                        if (auditList.Count() > 0)
                        {
                            HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                            {
                                HRMasterAuditTrailItems = auditList,
                                Type = "ApplicationRolePermission",
                                FormType = "Add",
                                HRMasterSetId = applicationrolepermission?.RoleID,
                                AuditUserId = applicationrolepermission?.AddedByUserID,
                            };
                            await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
                        }
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
        public async Task<List<ApplicationPermission>> GetByListSessionIdAsync(string SessionId)
        {
            try
            {
                var query = @"SELECT * FROM ApplicationPermission  WHERE UniqueSessionID = @SessionId";

                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId);

                using (var connection = CreateConnection())
                {
                    var res = await connection.QueryAsync<ApplicationPermission>(query, parameters);

                    return res.ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

    }
}
