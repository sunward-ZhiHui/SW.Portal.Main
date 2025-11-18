using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
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
                        var query = "select t1.*,t2.PermissionName from ApplicationRolePermission t1\r\nLEFT JOIN ApplicationPermission t2 ON t1.PermissionID=t2.PermissionID WHERE t1.RoleId= @RoleId;";

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
                var query = "Select  * from ApplicationPermission where 1=1 ORDER BY PermissionOrder";
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
                        if (added.Any() || removed.Any())
                        {
                            if (added.Any())
                            {
                                foreach (var item in added)
                                {
                                    if (item != 60000)
                                    {
                                        isUpdate = true;
                                        var uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRolePermission", "Add", null, item.ToString(), applicationrolepermission.RoleID, guid, applicationrolepermission?.AddedByUserID, DateTime.Now, false, "PermissionID", uid);
                                        uid = Guid.NewGuid();
                                        var names = results.FirstOrDefault(f => f.PermissionID == item)?.PermissionName;
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRolePermission", "Add", null, names, applicationrolepermission.RoleID, guid, applicationrolepermission?.AddedByUserID, DateTime.Now, false, "PermissionName", uid);

                                    }
                                }
                            }
                            if (removed.Any())
                            {
                                foreach (var item in removed)
                                {
                                    if (item != 60000)
                                    {
                                        isUpdate = true;
                                        var uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRolePermission", "Removed", item.ToString(), null, applicationrolepermission.RoleID, guid, applicationrolepermission?.AddedByUserID, DateTime.Now, false, "PermissionID", uid);
                                        uid = Guid.NewGuid();
                                        var names = results.FirstOrDefault(f => f.PermissionID == item)?.PermissionName;
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRolePermission", "Removed", names, null, applicationrolepermission.RoleID, guid, applicationrolepermission?.AddedByUserID, DateTime.Now, false, "PermissionName", uid);
                                    }
                                }
                            }
                        }
                        if (isUpdate)
                        {
                            var uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRolePermission", "Add/Update", applicationrolepermission?.RolePermissionName, applicationrolepermission?.RolePermissionName, applicationrolepermission?.RoleID, guid, applicationrolepermission?.AddedByUserID, DateTime.Now, false, "DisplayName", uid);
                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRolePermission", "Add/Update", applicationrolepermission?.AddedByUserID?.ToString(), applicationrolepermission?.AddedByUserID?.ToString(), applicationrolepermission?.RoleID, guid, applicationrolepermission?.AddedByUserID, DateTime.Now, false, "AddedByUserID", uid);
                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRolePermission", "Add/Update", applicationrolepermission?.AddedDate != null ? applicationrolepermission?.AddedDate.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, applicationrolepermission?.AddedDate != null ? applicationrolepermission?.AddedDate.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, applicationrolepermission?.RoleID, guid, applicationrolepermission?.AddedByUserID, DateTime.Now, false, "AddedDate", uid);
                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationRolePermission", "Add/Update", applicationrolepermission?.AddedBy?.ToString(), applicationrolepermission?.AddedBy?.ToString(), applicationrolepermission?.RoleID, guid, applicationrolepermission?.AddedByUserID, DateTime.Now, false, "AddedBy", uid);
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
