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
        public async Task<IReadOnlyList<HRMasterAuditTrail>> GetHRMasterAuditList(string? MasterType, long? MasterId, bool? IsDeleted, Guid? SessionId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("MasterType", MasterType, DbType.String);
                    parameters.Add("IsDeleted", IsDeleted);
                    parameters.Add("HRMasterSetId", MasterId);
                    var query = "select t1.*,t2.UserName as AuditUser from HRMasterAuditTrail t1 JOIN ApplicationUser t2 ON t2.UserId=t1.AuditUserId where t1.ColumnName not in('PlantId','CompanyId','DivisionID','StatusCodeID','ModifiedByUserID','SubSectionId','SectionID','DepartmentId','AddedByUserID','LevelId') AND t1.Type=@MasterType AND t1.IsDeleted=@IsDeleted\r";
                    if (IsDeleted == false)
                    {
                        query += "\rAND t1.HRMasterSetId=@HRMasterSetId\r";
                    }
                    query += "\rorder by t1.AuditDate desc\r";
                    try
                    {
                        return (await connection.QueryAsync<HRMasterAuditTrail>(query, parameters)).ToList();
                    }
                    catch (Exception exp)
                    {
                        throw (new ApplicationException(exp.Message));
                    }
                }
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }
        }
    }

}
