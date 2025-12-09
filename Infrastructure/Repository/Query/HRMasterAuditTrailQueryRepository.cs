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
using IdentityModel.Client;
using NAV;
using Application.Queries;
using Infrastructure.Data;
using static iText.IO.Image.Jpeg2000ImageData;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using System.Dynamic;
using System.Data.SqlClient;
using Newtonsoft.Json.Serialization;

namespace Infrastructure.Repository.Query
{
    public class HRMasterAuditTrailQueryRepository : DbConnector, IHRMasterAuditTrailQueryRepository
    {
        private readonly IAuditLogQueryRepository _auditLogQueryRepository;
        public HRMasterAuditTrailQueryRepository(IConfiguration configuration, IAuditLogQueryRepository auditLogQueryRepository)
            : base(configuration)
        {
            _auditLogQueryRepository = auditLogQueryRepository;
        }
        public DataTable ToDataTable(HRMasterAuditTrail? list)
        {
            list.SessionId = list.SessionId ?? Guid.NewGuid();
            var dt = new DataTable();
            dt.Columns.Add("UniqueSessionId", typeof(Guid));
            dt.Columns.Add("ColumnName", typeof(string));
            dt.Columns.Add("PreValue", typeof(string));
            dt.Columns.Add("CurrentValue", typeof(string));
            dt.Columns.Add("HRMasterSetID", typeof(long));
            dt.Columns.Add("SessionID", typeof(Guid));
            dt.Columns.Add("AuditUserID", typeof(long));
            dt.Columns.Add("AuditDate", typeof(DateTime));
            dt.Columns.Add("IsDeleted", typeof(bool));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("FormType", typeof(string));

            foreach (var a in list.HRMasterAuditTrailItems)
            {
                dt.Rows.Add(
                    a.UniqueSessionId != null ? a.UniqueSessionId : Guid.NewGuid(),
                    a.ColumnName,
                    a.PreValue,
                    a.CurrentValue,
                    list.HRMasterSetId,
                    list.SessionId,
                    list.AuditUserId,
                    list.AuditDate != null ? list.AuditDate : DateTime.Now,
                    list.IsDeleted ?? false,
                    list.Type,
                    !string.IsNullOrEmpty(a.FormType) ? a.FormType : list.FormType

                );
            }
            return dt;
        }
        public async Task BulkInsertAudit(HRMasterAuditTrail auditList)
        {
            try
            {
                if (auditList.HRMasterAuditTrailItems?.Any() == true)
                {
                    var dt = ToDataTable(auditList);

                    using (var connection = (SqlConnection)CreateConnection())
                    {
                        await connection.OpenAsync();

                        using (var bulk = new SqlBulkCopy(connection))
                        {
                            bulk.DestinationTableName = "HRMasterAuditTrail";

                            bulk.ColumnMappings.Add("UniqueSessionId", "UniqueSessionId");
                            bulk.ColumnMappings.Add("ColumnName", "ColumnName");
                            bulk.ColumnMappings.Add("PreValue", "PreValue");
                            bulk.ColumnMappings.Add("CurrentValue", "CurrentValue");
                            bulk.ColumnMappings.Add("HRMasterSetId", "HRMasterSetID");
                            bulk.ColumnMappings.Add("SessionId", "SessionID");
                            bulk.ColumnMappings.Add("AuditUserId", "AuditUserID");
                            bulk.ColumnMappings.Add("AuditDate", "AuditDate");
                            bulk.ColumnMappings.Add("IsDeleted", "IsDeleted");
                            bulk.ColumnMappings.Add("Type", "Type");
                            bulk.ColumnMappings.Add("FormType", "FormType");
                            await bulk.WriteToServerAsync(dt);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                throw new NotImplementedException();
            }
        }

        public async Task InsertHRMasterAuditTrail(string? Type, string? FormType, string? PreValue, string? CurrentValue, long? HRMasterSetID, Guid? SessionId, long? AuditUserId, DateTime? AuditDate, bool? IsDeleted, string? columnName, Guid? UniqueSessionId = null)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {

                        var parameters = new DynamicParameters();
                        parameters.Add("PreValue", PreValue, DbType.String);
                        parameters.Add("CurrentValue", CurrentValue, DbType.String);
                        parameters.Add("HRMasterSetID", HRMasterSetID);
                        parameters.Add("SessionId", SessionId, DbType.Guid);
                        parameters.Add("AuditUserId", AuditUserId);
                        parameters.Add("AuditDate", AuditDate, DbType.DateTime);
                        parameters.Add("IsDeleted", IsDeleted == true ? true : false);
                        parameters.Add("Type", Type, DbType.String);
                        parameters.Add("FormType", FormType, DbType.String);
                        parameters.Add("ColumnName", columnName, DbType.String);
                        parameters.Add("UniqueSessionId", UniqueSessionId, DbType.Guid);
                        var query = "INSERT INTO HRMasterAuditTrail(UniqueSessionId,ColumnName,PreValue,CurrentValue,HRMasterSetID,SessionId,AuditUserId,AuditDate,IsDeleted,Type,FormType)  " +
                            "OUTPUT INSERTED.HRMasterAuditTrailID VALUES " +
                            "(@UniqueSessionId,@ColumnName,@PreValue,@CurrentValue,@HRMasterSetID,@SessionId,@AuditUserId,@AuditDate,@IsDeleted,@Type,@FormType)";
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);

                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }

                }


            }
            catch (Exception exp)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<IReadOnlyList<HRMasterAuditTrail>> GetHRMasterAuditList(string? MasterType, long? MasterId, bool? IsDeleted, Guid? SessionId, string? AddTypeId = "")
        {
            try
            {
                List<HRMasterAuditTrail> HRMasterAuditTrail = new List<HRMasterAuditTrail>();
                using (var connection = CreateConnection())
                {
                    string query1 = "\rt1.ColumnName NOT LIKE '%Id'\r";
                    if (!string.IsNullOrEmpty(MasterType))
                    {
                        var names = MasterType.ToLower().Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

                        if (names?.Any() == true && names.Contains("applicationrolepermission"))
                        {
                            query1 = "\r(t1.ColumnName NOT LIKE '%Id' OR t1.ColumnName='PermissionID') AND t1.ColumnName!='PermissionName' \r";
                        }
                    }
                    var masterTypes = MasterType?.Split(",").ToList();
                    var parameters = new DynamicParameters();
                    parameters.Add("@MasterType", masterTypes);
                    parameters.Add("IsDeleted", IsDeleted);
                    parameters.Add("HRMasterSetId", MasterId);
                    var query = "select t1.*,t2.UserName as AuditUser from HRMasterAuditTrail t1 JOIN ApplicationUser t2 ON t2.UserId=t1.AuditUserId where " + query1 + " AND t1.ColumnName not in('ReportToIds','PlantId','CompanyId','DivisionID','StatusCodeID','ModifiedByUserID','SubSectionId','SectionID','DepartmentId','AddedByUserID','LevelId','TypeOfEmployeement','LanguageID','RoleID','PlantID','DesignationID','SectionID','SubSectionID','LevelID','AcceptanceStatus','DynamicFormId','ProfileId','ParentId','UserId','ShelfLifeDurationID','DocumentRoleId') AND t1.Type IN @MasterType AND t1.IsDeleted=@IsDeleted\r";
                    if (IsDeleted == false)
                    {
                        query += "\rAND t1.HRMasterSetId=@HRMasterSetId\r";
                    }
                    query += "\rorder by t1.AuditDate desc\r";
                    try
                    {
                        HRMasterAuditTrail = (await connection.QueryAsync<HRMasterAuditTrail>(query, parameters)).ToList();

                    }
                    catch (Exception exp)
                    {
                        throw (new ApplicationException(exp.Message));
                    }
                }
                if (HRMasterAuditTrail?.Any() == true && !string.IsNullOrEmpty(AddTypeId))
                {
                    var ids = HRMasterAuditTrail.FirstOrDefault()?.HRMasterSetId;
                    if (ids > 0)
                    {
                        var result = await GetHRMasterAuditSubList(MasterType, ids, AddTypeId);
                        if (result != null)
                        {
                            HRMasterAuditTrail.AddRange(result);
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(AddTypeId))
                    {
                        HRMasterAuditTrail = new List<HRMasterAuditTrail>();
                        var result = await GetHRMasterAuditSubList(MasterType, MasterId, AddTypeId);
                        if (result != null)
                        {
                            HRMasterAuditTrail.AddRange(result);
                        }
                    }
                }
                return HRMasterAuditTrail;
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }
        }
        public async Task<List<HRMasterAuditTrail>> GetHRMasterAuditSubList(string? MasterType, long? MasterId, string? AddTypeId = "")
        {
            try
            {
                List<HRMasterAuditTrail> HRMasterAuditTrail = new List<HRMasterAuditTrail>();
                using (var connection = CreateConnection())
                {
                    var masterTypes = MasterType?.Split(",").ToList();
                    var parameters = new DynamicParameters();
                    parameters.Add("@MasterType", masterTypes);
                    parameters.Add("AddTypeId", AddTypeId, DbType.String);
                    parameters.Add("HRMasterSetId", MasterId);
                    var query = "select t1.* from HRMasterAuditTrail t1  where t1.HRMasterSetID IN(select t2.HRMasterSetID from HRMasterAuditTrail t2 where t2.ColumnName=@AddTypeId  AND (t2.CurrentValue=@HRMasterSetId OR t2.PreValue=@HRMasterSetId) group by t2.HRMasterSetID \r\n)AND t1.ColumnName NOT LIKE '%Id' order by t1.AuditDate desc";
                    try
                    {
                        HRMasterAuditTrail = (await connection.QueryAsync<HRMasterAuditTrail>(query, parameters)).ToList();

                    }
                    catch (Exception exp)
                    {
                        throw (new ApplicationException(exp.Message));
                    }
                }
                return HRMasterAuditTrail;
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }
        }
        public async Task<IReadOnlyList<FileProfileTypeModel>> GetHRMasterSWAuditList(string? MasterType, bool? IsDeleted)
        {
            try
            {
                List<FileProfileTypeModel> HRMasterAuditTrail = new List<FileProfileTypeModel>();
                using (var connection = CreateConnection())
                {
                    var masterTypes = MasterType?.Split(",").ToList();
                    var parameters = new DynamicParameters();
                    parameters.Add("MasterType", MasterType);
                    var query = "select tt1.HRMasterSetID as FileProfileTypeID,t2.Name as Name,t2.ParentID,t2.Description,t3.UserName as AddedByUser,t4.UserName as ModifiedByUser,t2.AddedDate,t3.ModifiedDate from(select t1.HRMasterSetID from HRMasterAuditTrail t1 where t1.Type=@MasterType AND (t1.IsDeleted is null OR t1.IsDeleted=0) group By t1.HRMasterSetID)tt1\r\nLEFT JOIN FileProfileType t2 ON t2.FileProfileTypeID=tt1.HRMasterSetID\r\nLEFT JOIN ApplicationUser t3 ON t3.UserID=t2.AddedByUserID\r\nLEFT JOIN ApplicationUser t4 ON t4.UserID=t2.ModifiedByUserID";
                    try
                    {
                        HRMasterAuditTrail = (await connection.QueryAsync<FileProfileTypeModel>(query, parameters)).ToList();

                    }
                    catch (Exception exp)
                    {
                        throw (new ApplicationException(exp.Message));
                    }
                }
                return HRMasterAuditTrail;
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }
        }
        public async Task<IReadOnlyList<ApplicationPermission>> GetHRMasterApplicationPermissionAuditList()
        {
            try
            {
                List<ApplicationPermission> HRMasterAuditTrail = new List<ApplicationPermission>();
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    var query = "select * from ApplicationPermission where PermissionID>60000";
                    try
                    {
                        HRMasterAuditTrail = (await connection.QueryAsync<ApplicationPermission>(query, parameters)).ToList();

                    }
                    catch (Exception exp)
                    {
                        throw (new ApplicationException(exp.Message));
                    }
                }
                return HRMasterAuditTrail;
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }
        }
    }
}
