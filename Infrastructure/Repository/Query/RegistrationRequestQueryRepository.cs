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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Dynamic;
using Newtonsoft.Json;
using static iTextSharp.text.pdf.AcroFields;
using Newtonsoft.Json.Linq;
using System.Net.Http.Json;
using Org.BouncyCastle.Ocsp;

namespace Infrastructure.Repository.Query
{
    public class RegistrationRequestQueryRepository : DbConnector, IRegistrationRequestQueryRepository
    {
        private readonly IGenerateDocumentNoSeriesSeviceQueryRepository _generateDocumentNoSeriesSeviceQueryRepository;
        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        private readonly HttpClient _httpClient;
        private readonly IEmailConversationsQueryRepository _emailConversationsQueryRepository;
        private readonly IProductionActivityAppQueryRepository _productionActivityAppQueryRepository;
        public RegistrationRequestQueryRepository(IConfiguration configuration, IGenerateDocumentNoSeriesSeviceQueryRepository generateDocumentNoSeriesSeviceQueryRepository, IEmailTopicsQueryRepository emailTopicsQueryRepository, HttpClient httpClient, IEmailConversationsQueryRepository emailConversationsQueryRepository, IProductionActivityAppQueryRepository productionActivityAppQueryRepository)
            : base(configuration)
        {
            _generateDocumentNoSeriesSeviceQueryRepository = generateDocumentNoSeriesSeviceQueryRepository;
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
            _httpClient = httpClient;
            _emailConversationsQueryRepository = emailConversationsQueryRepository;
            _productionActivityAppQueryRepository = productionActivityAppQueryRepository;
        }
        public async Task<IReadOnlyList<RegistrationRequest>> GetRegistrationRequest()
        {
            try
            {
                var result = new List<RegistrationRequest>();
                var resultData = new List<RegistrationRequestVariation>();
                var query = "\r\nselect t1.*,t2.Value as RegistrationCountry,t3.UserName as AddedBy,t4.UserName as ModifiedBy,t5.Name as ProfileName from RegistrationRequest t1  JOIN ApplicationMasterChild t2 ON t1.RegistrationCountryID=t2.ApplicationMasterChildID JOIN ApplicationUser t3 ON t3.UserID=t1.AddedByUserID \r\nLEFT JOIN ApplicationUser t4 ON t4.UserID=t1.ModifiedUserID \r\nLEFT JOIN DocumentProfileNoSeries t5 ON t5.ProfileID=t1.ProfileID \r\nwhere (t1.Isdeleted is null Or t1.Isdeleted=0);";
                query += "select * from RegistrationRequestVariation;";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query);
                    result = results.ReadAsync<RegistrationRequest>().Result.ToList();
                    resultData = results.ReadAsync<RegistrationRequestVariation>().Result.ToList();
                }
                if (resultData.Count > 0)
                {
                    result.ForEach(x =>
                    {
                        x.VariationNoIds = resultData.Where(w => w.RegistrationRequestId == x.RegistrationRequestId).Select(s => s.DynamicFormDataId).ToList();
                    });
                }
                return result;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<RegistrationRequest> GetRegistrationRequestBySession(Guid? SessionId)
        {
            try
            {
                var result = new RegistrationRequest();
                var resultData = new List<RegistrationRequestVariation>();
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                var query = "select t1.*,t2.Value as RegistrationCountry,t3.UserName as AddedBy,t4.UserName as ModifiedBy,t5.Name as ProfileName from RegistrationRequest t1  JOIN ApplicationMasterChild t2 ON t1.RegistrationCountryID=t2.ApplicationMasterChildID JOIN ApplicationUser t3 ON t3.UserID=t1.AddedByUserID \r\nLEFT JOIN ApplicationUser t4 ON t4.UserID=t1.ModifiedUserID \r\nLEFT JOIN DocumentProfileNoSeries t5 ON t5.ProfileID=t1.ProfileID \r\nwhere (t1.Isdeleted is null Or t1.Isdeleted=0) AND t1.SessionId=@SessionId;";
                query += "select * from RegistrationRequestVariation;";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    result = results.ReadAsync<RegistrationRequest>().Result.FirstOrDefault();
                    resultData = results.ReadAsync<RegistrationRequestVariation>().Result.ToList();
                }
                if (result != null && result.RegistrationRequestId > 0)
                {
                    result.VariationNoIds = resultData.Where(w => w.RegistrationRequestId == result.RegistrationRequestId).Select(s => s.DynamicFormDataId).ToList();
                }
                return result;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<RegistrationRequest> DeleteRegistrationRequest(RegistrationRequest value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("RegistrationRequestId", value.RegistrationRequestId);
                        var query = "Update  RegistrationRequest SET IsDeleted=1 WHERE RegistrationRequestId = @RegistrationRequestId";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        return value;
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
        public async Task<IReadOnlyList<RegistrationRequestDueDateAssignment>> GetRegistrationRequestDueDateAssignment(long? RegistrationRequestId)
        {
            try
            {
                var result = new List<RegistrationRequestDueDateAssignment>();
                var parameters = new DynamicParameters();
                parameters.Add("RegistrationRequestId", RegistrationRequestId);
                var query = "select t1.*,t3.UserName as AddedBy,t4.UserName as ModifiedBy,tt2.Name as DepartmentName from RegistrationRequestDueDateAssignment t1\r\nJOIN RegistrationRequest t2 ON t1.RegistrationRequestID=t2.RegistrationRequestID\r\nLEFT JOIN Department tt2 ON t1.DepartmentID=tt2.DepartmentID\r\nJOIN ApplicationUser t3 ON t3.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t4 ON t4.UserID=t1.ModifiedUserID Where t1.RegistrationRequestId=@RegistrationRequestId;";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    result = results.ReadAsync<RegistrationRequestDueDateAssignment>().Result.ToList();
                }
                return result;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<RegistrationRequestDueDateAssignment> DeleteRegistrationRequestDueDateAssignment(RegistrationRequestDueDateAssignment value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("RegistrationRequestDueDateAssignmentId", value.RegistrationRequestDueDateAssignmentId);
                        var query = "Delete from RegistrationRequestDueDateAssignment  WHERE RegistrationRequestDueDateAssignmentId = @RegistrationRequestDueDateAssignmentId";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        return value;
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
        public async Task<IReadOnlyList<RegistrationRequestVariationForm>> GetRegistrationRequestVariationForm(long? RegistrationRequestId)
        {
            try
            {
                List<RegistrationRequestVariationForm> result = new List<RegistrationRequestVariationForm>();
                var parameters = new DynamicParameters();
                parameters.Add("RegistrationRequestId", RegistrationRequestId);

                var query = "select t1.*,t2.DynamicFormDataID as RegDynamicFormDataID,(CONCAT(t2.DynamicFormDataID,'_',t1.DynamicFormDataID,'_Id')) as RegDynamicFormDataNameID  from RegistrationRequestVariationForm t1 JOIN RegistrationRequestVariation t2 ON t1.RegistrationRequestVariationID=t2.RegistrationRequestVariationID Where t2.RegistrationRequestId = @RegistrationRequestId";
                using (var connection = CreateConnection())
                {
                    var res = (await connection.QueryAsync<RegistrationRequestVariationForm>(query, parameters)).ToList();
                    result = res != null ? res : new List<RegistrationRequestVariationForm>();
                }
                return result;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<RegistrationRequestDepartment>> GeRegistrationRequestDepartment(long? RegistrationRequestId)
        {
            try
            {
                List<RegistrationRequestDepartment> result = new List<RegistrationRequestDepartment>();
                var parameters = new DynamicParameters();
                parameters.Add("RegistrationRequestId", RegistrationRequestId);

                var query = "select t1.* from RegistrationRequestDepartment t1  Where t1.RegistrationRequestId = @RegistrationRequestId";
                using (var connection = CreateConnection())
                {
                    var res = (await connection.QueryAsync<RegistrationRequestDepartment>(query, parameters)).ToList();
                    result = res != null ? res : new List<RegistrationRequestDepartment>();
                }
                return result;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<RegistrationRequest> InsertorUpdateRegistrationRequest(RegistrationRequest value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("RegistrationCountryId", value.RegistrationCountryId);
                    parameters.Add("CCNo", value.CCNo, DbType.String);
                    parameters.Add("SessionId", value.SessionId, DbType.Guid);
                    parameters.Add("ProductSpecificationDynamicFormId", value.ProductSpecificationDynamicFormId);
                    parameters.Add("ExpectedSubmissionDate", value.ExpectedSubmissionDate, DbType.DateTime);
                    parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                    parameters.Add("AddedByUserId", value.AddedByUserId);
                    parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                    parameters.Add("ModifiedUserId", value.ModifiedUserId);
                    parameters.Add("SubmissionDate", value.SubmissionDate, DbType.DateTime);
                    parameters.Add("SubmissionNo", value.SubmissionNo, DbType.String);
                    parameters.Add("PurposeOfRegistration", value.PurposeOfRegistration, DbType.String);
                    parameters.Add("StatusCodeId", value.StatusCodeId);
                    parameters.Add("IsAllowNA", value.IsAllowNA);
                    parameters.Add("RegistrationRequestId", value.RegistrationRequestId);
                    parameters.Add("ProfileId", value.ProfileId);
                    if (value.RegistrationRequestId > 0)
                    {
                        var ProfileNo = value.ProfileNo;
                        if (value.PreProfileId > 0)
                        {
                        }
                        else
                        {
                            ProfileNo = await _generateDocumentNoSeriesSeviceQueryRepository.GenerateDocumentProfileAutoNumber(new DocumentNoSeriesModel { ProfileID = value.ProfileId, Title = "Registration Request", StatusCodeID = 710 });
                            value.ProfileNo = ProfileNo;
                        }
                        parameters.Add("ProfileNo", ProfileNo, DbType.String);
                        var query = "UPDATE RegistrationRequest SET IsAllowNA=@IsAllowNA,ProfileNo=@ProfileNo,ProfileId=@ProfileId,PurposeOfRegistration=@PurposeOfRegistration,SubmissionNo=@SubmissionNo,RegistrationCountryId=@RegistrationCountryId,CCNo = @CCNo,ProductSpecificationDynamicFormId =@ProductSpecificationDynamicFormId,SessionId =@SessionId,ModifiedUserId=@ModifiedUserId," +
                            "ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID,ExpectedSubmissionDate=@ExpectedSubmissionDate WHERE RegistrationRequestId = @RegistrationRequestId";

                        await connection.ExecuteAsync(query, parameters);
                    }
                    else
                    {
                        var ProfileNo = await _generateDocumentNoSeriesSeviceQueryRepository.GenerateDocumentProfileAutoNumber(new DocumentNoSeriesModel { ProfileID = value.ProfileId, Title = "Registration Request", StatusCodeID = 710 });
                        value.ProfileNo = ProfileNo;
                        parameters.Add("ProfileNo", ProfileNo, DbType.String);
                        var query = "INSERT INTO RegistrationRequest(IsAllowNA,ProfileId,ProfileNo,PurposeOfRegistration,SubmissionNo,RegistrationCountryId,CCNo,ProductSpecificationDynamicFormId,SessionId,AddedByUserId,AddedDate,ModifiedUserId,ModifiedDate,SubmissionDate,StatusCodeId,ExpectedSubmissionDate) OUTPUT INSERTED.RegistrationRequestId VALUES " +
                            "(@IsAllowNA,@ProfileId,@ProfileNo,@PurposeOfRegistration,@SubmissionNo,@RegistrationCountryId,@CCNo,@ProductSpecificationDynamicFormId,@SessionId,@AddedByUserId,@AddedDate,@ModifiedUserId,@ModifiedDate,@SubmissionDate,@StatusCodeId,@ExpectedSubmissionDate)";

                        var rowsAffected = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        value.RegistrationRequestId = rowsAffected;
                    }
                    await InsertRegistrationRequestVariation(value);
                    await InsertRegistrationRequestAssignmentOfJob(value);

                    /*var querys = string.Empty;
                    querys += "Delete from RegistrationRequestVariation where RegistrationRequestID=" + value.RegistrationRequestId + ";\r\n";
                    if (value.VariationNoIds != null && value.VariationNoIds.Count() > 0)
                    {
                        value.VariationNoIds.ForEach(s =>
                        {
                            querys += "INSERT INTO RegistrationRequestVariation(RegistrationRequestID,DynamicFormDataID) VALUES " +
                            "(" + value.RegistrationRequestId + "," + s + ");\n\r";
                        });
                    }
                    if (!string.IsNullOrEmpty(querys))
                    {
                        var rowsAffected = await connection.ExecuteAsync(querys);
                    }*/
                    return value;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<RegistrationRequestAssignmentOfJob> InsertRegistrationRequestAssignmentOfJob(RegistrationRequest registrationRequest)
        {
            try
            {
                RegistrationRequestAssignmentOfJob registrationRequestAssignmentOfJob = new RegistrationRequestAssignmentOfJob();
                var resultData = await GetRegistrationRequestVariationForm(registrationRequest.RegistrationRequestId);
                var departmentIds = registrationRequest.VariationRequirementInformationModels.Select(d => d.DepartmentId).Distinct().ToList();
                var deprtList = await GeRegistrationRequestDepartment(registrationRequest.RegistrationRequestId);
                using (var connection = CreateConnection())
                {
                    if (departmentIds != null && departmentIds.Count() > 0)
                    {
                        List<long> RegistrationRequestDepartmentIds = new List<long>();
                        foreach (var departmentId in departmentIds)
                        {
                            var parameters2 = new DynamicParameters();
                            parameters2.Add("RegistrationRequestId", registrationRequest.RegistrationRequestId);
                            parameters2.Add("DepartmentId", departmentId);
                            var exitsData = deprtList.Where(w => w.RegistrationRequestId == registrationRequest.RegistrationRequestId && w.DepartmentId == departmentId).FirstOrDefault();
                            if (exitsData != null)
                            {
                                RegistrationRequestDepartmentIds.Add(exitsData.RegistrationRequestDepartmentId);
                            }
                            else
                            {
                                parameters2.Add("DepartmentUniqueSessionID", Guid.NewGuid(), DbType.Guid);
                                var query1 = "INSERT INTO RegistrationRequestDepartment(DepartmentId,DepartmentUniqueSessionID,RegistrationRequestId) OUTPUT INSERTED.RegistrationRequestDepartmentId VALUES " +
                                "(@DepartmentId,@DepartmentUniqueSessionID,@RegistrationRequestId)";
                                var rowsAffected1 = await connection.QuerySingleOrDefaultAsync<long>(query1, parameters2);
                            }
                        }
                        List<long> deprtListIds = new List<long>();
                        deprtListIds.AddRange(deprtList.Select(s => s.RegistrationRequestDepartmentId).ToList());
                        var excelpt = deprtListIds.Except(RegistrationRequestDepartmentIds).ToList();
                        if (excelpt.Count > 0)
                        {
                            var querys = string.Empty;
                            querys += "Delete from RegistrationRequestDepartment where RegistrationRequestId=" + registrationRequest.RegistrationRequestId + " AND RegistrationRequestDepartmentId in(" + string.Join(",", excelpt) + ");\r\n";

                            if (!string.IsNullOrEmpty(querys))
                            {
                                var rowsAffected = await connection.ExecuteAsync(querys);
                            }
                        }
                    }
                    else
                    {
                        var querys = string.Empty;
                        querys += "Delete from RegistrationRequestDepartment where RegistrationRequestId=" + registrationRequest.RegistrationRequestId + ";\r\n";
                        if (!string.IsNullOrEmpty(querys))
                        {
                            var rowsAffected = await connection.ExecuteAsync(querys);
                        }
                    }
                    var parameters1 = new DynamicParameters();
                    parameters1.Add("RegistrationRequestId", registrationRequest.RegistrationRequestId);
                    var query = "select *  from RegistrationRequestAssignmentOfJob  WHERE RegistrationRequestId = @RegistrationRequestId";
                    var RegistrationResult = (await connection.QueryAsync<RegistrationRequestAssignmentOfJob>(query, parameters1)).ToList();
                    RegistrationResult = RegistrationResult != null ? RegistrationResult.ToList() : new List<RegistrationRequestAssignmentOfJob>();
                    var RegistrationResultIds = RegistrationResult.Select(s => s.DynamicFormDataId).Distinct().ToList();
                    if (RegistrationResultIds.Count > 0)
                    {
                        if (resultData.Count() == 0)
                        {
                            var querys = string.Empty;
                            querys += "Delete from RegistrationRequestAssignmentOfJob where RegistrationRequestId=" + registrationRequest.RegistrationRequestId + " AND DynamicFormDataId in(" + string.Join(",", RegistrationResultIds) + ");\r\n";
                            if (!string.IsNullOrEmpty(querys))
                            {
                                var rowsAffected = await connection.ExecuteAsync(querys);
                            }
                        }
                    }
                    if (resultData != null && resultData.Count() > 0)
                    {
                        var dynamicIds = resultData.Select(s => s.DynamicFormDataId).Distinct().ToList();
                        var excelpt = RegistrationResultIds.Except(dynamicIds).ToList();
                        if (excelpt.Count > 0)
                        {
                            var querys = string.Empty;
                            querys += "Delete from RegistrationRequestAssignmentOfJob where RegistrationRequestId=" + registrationRequest.RegistrationRequestId + " AND DynamicFormDataId in(" + string.Join(",", excelpt) + ");\r\n";

                            if (!string.IsNullOrEmpty(querys))
                            {
                                var rowsAffected = await connection.ExecuteAsync(querys);
                            }
                        }
                        if (dynamicIds != null && dynamicIds.Count() > 0)
                        {
                            var deprtAgainList = await GeRegistrationRequestDepartment(registrationRequest.RegistrationRequestId);
                            foreach (var d in dynamicIds)
                            {
                                var data = resultData.Where(w => w.DynamicFormDataId == d).ToList();
                                if (data != null && data.Count() > 0)
                                {
                                    var names = registrationRequest.VariationRequirementInformationModels.FirstOrDefault(f => f.DynamicFormDataId == d);
                                    var RegistrationRequestDepartmentIds = deprtAgainList.FirstOrDefault(f => f.DepartmentId == names?.DepartmentId && f.RegistrationRequestId == registrationRequest.RegistrationRequestId).RegistrationRequestDepartmentId;
                                    var parameters = new DynamicParameters();
                                    parameters.Add("DetailInforamtionByGuideline", data.FirstOrDefault(f => f.DynamicFormDataId == d && f.DetailType == "DetailInfo")?.Description, DbType.String);
                                    parameters.Add("DetailRequirement", data.FirstOrDefault(f => f.DynamicFormDataId == d && f.DetailType == "DetailRequirement")?.Description, DbType.String);
                                    parameters.Add("SessionId", Guid.NewGuid(), DbType.Guid);
                                    parameters.Add("AddedDate", registrationRequest.AddedDate, DbType.DateTime);
                                    parameters.Add("AddedByUserId", registrationRequest.AddedByUserId);
                                    parameters.Add("ModifiedDate", registrationRequest.AddedDate, DbType.DateTime);
                                    parameters.Add("ModifiedUserId", registrationRequest.AddedByUserId);
                                    parameters.Add("DepartmentId", names?.DepartmentId);
                                    parameters.Add("StatusCodeId", 1);
                                    parameters.Add("DynamicFormDataId", d);
                                    parameters.Add("RegistrationRequestId", registrationRequest.RegistrationRequestId);
                                    parameters.Add("JobNo", names?.DescriptionName, DbType.String);
                                    parameters.Add("RegistrationRequestDepartmentId", RegistrationRequestDepartmentIds);
                                    var exitsData = RegistrationResult.FirstOrDefault(q => q.DynamicFormDataId == d && q.RegistrationRequestId == registrationRequest.RegistrationRequestId);
                                    if (exitsData != null)
                                    {
                                        parameters.Add("IsEmailCreateDone", exitsData.IsEmailCreateDone == true ? true : null);
                                        parameters.Add("RegistrationRequestAssignmentOfJobId", exitsData.RegistrationRequestAssignmentOfJobId);
                                        var query1 = "UPDATE RegistrationRequestAssignmentOfJob SET IsEmailCreateDone=@IsEmailCreateDone,RegistrationRequestDepartmentId=@RegistrationRequestDepartmentId,DynamicFormDataId=@DynamicFormDataId,RegistrationRequestId=@RegistrationRequestId,JobNo=@JobNo,DetailRequirement=@DetailRequirement,DetailInforamtionByGuideline=@DetailInforamtionByGuideline,DepartmentId=@DepartmentId," +
                                            "ModifiedUserId=@ModifiedUserId,ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID WHERE RegistrationRequestAssignmentOfJobId = @RegistrationRequestAssignmentOfJobId";

                                        await connection.ExecuteAsync(query1, parameters);
                                    }
                                    else
                                    {
                                        var query1 = "INSERT INTO RegistrationRequestAssignmentOfJob(RegistrationRequestDepartmentId,DynamicFormDataId,JobNo,DetailRequirement,RegistrationRequestId,DetailInforamtionByGuideline,DepartmentId,SessionId,AddedByUserId,AddedDate,ModifiedUserId,ModifiedDate,StatusCodeId) OUTPUT INSERTED.RegistrationRequestAssignmentOfJobId VALUES " +
                                        "(@RegistrationRequestDepartmentId,@DynamicFormDataId,@JobNo,@DetailRequirement,@RegistrationRequestId,@DetailInforamtionByGuideline,@DepartmentId,@SessionId,@AddedByUserId,@AddedDate,@ModifiedUserId,@ModifiedDate,@StatusCodeId)";

                                        var rowsAffected1 = await connection.QuerySingleOrDefaultAsync<long>(query1, parameters);
                                    }
                                }
                            }
                        }
                    }
                }
                return registrationRequestAssignmentOfJob;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        /*public async Task<RegistrationRequestAssignmentOfJob> InsertRegistrationRequestAssignmentOfJob(RegistrationRequest registrationRequest)
        {
            try
            {
                RegistrationRequestAssignmentOfJob registrationRequestAssignmentOfJob = new RegistrationRequestAssignmentOfJob();
                var resultData = await GetRegistrationRequestVariationForm(registrationRequest.RegistrationRequestId);
                using (var connection = CreateConnection())
                {
                    var parameters1 = new DynamicParameters();
                    parameters1.Add("RegistrationRequestId", registrationRequest.RegistrationRequestId);
                    var query = "select *  from RegistrationRequestAssignmentOfJob  WHERE RegistrationRequestId = @RegistrationRequestId";
                    var RegistrationResult = (await connection.QueryAsync<RegistrationRequestAssignmentOfJob>(query, parameters1)).ToList();
                    RegistrationResult = RegistrationResult != null ? RegistrationResult.ToList() : new List<RegistrationRequestAssignmentOfJob>();
                    var RegistrationResultIds = RegistrationResult.Select(s => s.DynamicFormDataId).Distinct().ToList();
                    if (RegistrationResultIds.Count > 0)
                    {
                        if (resultData.Count() == 0)
                        {
                            var querys = string.Empty;
                            querys += "Delete from RegistrationRequestAssignmentOfJob where RegistrationRequestId=" + registrationRequest.RegistrationRequestId + " AND DynamicFormDataId in(" + string.Join(",", RegistrationResultIds) + ");\r\n";
                            if (!string.IsNullOrEmpty(querys))
                            {
                                var rowsAffected = await connection.ExecuteAsync(querys);
                            }
                        }
                    }
                    if (resultData != null && resultData.Count() > 0)
                    {
                        var dynamicIds = resultData.Select(s => s.DynamicFormDataId).Distinct().ToList();
                        var excelpt = RegistrationResultIds.Except(dynamicIds).ToList();
                        if (excelpt.Count > 0)
                        {
                            var querys = string.Empty;
                            querys += "Delete from RegistrationRequestAssignmentOfJob where RegistrationRequestId=" + registrationRequest.RegistrationRequestId + " AND DynamicFormDataId in(" + string.Join(",", excelpt) + ");\r\n";

                            if (!string.IsNullOrEmpty(querys))
                            {
                                var rowsAffected = await connection.ExecuteAsync(querys);
                            }
                        }
                        if (dynamicIds != null && dynamicIds.Count() > 0)
                        {
                            foreach (var d in dynamicIds)
                            {
                                var data = resultData.Where(w => w.DynamicFormDataId == d).ToList();
                                if (data != null && data.Count() > 0)
                                {
                                    var names = registrationRequest.VariationRequirementInformationModels.FirstOrDefault(f => f.DynamicFormDataId == d);
                                    var parameters = new DynamicParameters();
                                    parameters.Add("DetailInforamtionByGuideline", data.FirstOrDefault(f => f.DynamicFormDataId == d && f.DetailType == "DetailInfo")?.Description, DbType.String);
                                    parameters.Add("DetailRequirement", data.FirstOrDefault(f => f.DynamicFormDataId == d && f.DetailType == "DetailRequirement")?.Description, DbType.String);
                                    parameters.Add("SessionId", Guid.NewGuid(), DbType.Guid);
                                    parameters.Add("AddedDate", registrationRequest.AddedDate, DbType.DateTime);
                                    parameters.Add("AddedByUserId", registrationRequest.AddedByUserId);
                                    parameters.Add("ModifiedDate", registrationRequest.AddedDate, DbType.DateTime);
                                    parameters.Add("ModifiedUserId", registrationRequest.AddedByUserId);
                                    parameters.Add("DepartmentId", names?.DepartmentId);
                                    parameters.Add("StatusCodeId", 1);
                                    parameters.Add("DynamicFormDataId", d);
                                    parameters.Add("RegistrationRequestId", registrationRequest.RegistrationRequestId);
                                    parameters.Add("JobNo", names?.DescriptionName, DbType.String);
                                    var exitsData = RegistrationResult.FirstOrDefault(q => q.DynamicFormDataId == d && q.RegistrationRequestId == registrationRequest.RegistrationRequestId);
                                    if (exitsData != null)
                                    {
                                        parameters.Add("RegistrationRequestAssignmentOfJobId", exitsData.RegistrationRequestAssignmentOfJobId);
                                        var query1 = "UPDATE RegistrationRequestAssignmentOfJob SET DynamicFormDataId=@DynamicFormDataId,RegistrationRequestId=@RegistrationRequestId,JobNo=@JobNo,DetailRequirement=@DetailRequirement,DetailInforamtionByGuideline=@DetailInforamtionByGuideline,DepartmentId=@DepartmentId," +
                                            "ModifiedUserId=@ModifiedUserId,ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID WHERE RegistrationRequestAssignmentOfJobId = @RegistrationRequestAssignmentOfJobId";

                                        await connection.ExecuteAsync(query1, parameters);
                                    }
                                    else
                                    {
                                        var query1 = "INSERT INTO RegistrationRequestAssignmentOfJob(DynamicFormDataId,JobNo,DetailRequirement,RegistrationRequestId,DetailInforamtionByGuideline,DepartmentId,SessionId,AddedByUserId,AddedDate,ModifiedUserId,ModifiedDate,StatusCodeId) OUTPUT INSERTED.RegistrationRequestAssignmentOfJobId VALUES " +
                                        "(@DynamicFormDataId,@JobNo,@DetailRequirement,@RegistrationRequestId,@DetailInforamtionByGuideline,@DepartmentId,@SessionId,@AddedByUserId,@AddedDate,@ModifiedUserId,@ModifiedDate,@StatusCodeId)";

                                        var rowsAffected1 = await connection.QuerySingleOrDefaultAsync<long>(query1, parameters);
                                    }
                                }
                            }
                        }
                    }
                }
                return registrationRequestAssignmentOfJob;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }*/
        public async Task<IReadOnlyList<RegistrationRequestVariation>> InsertRegistrationRequestVariation(RegistrationRequest registrationRequest)
        {
            try
            {
                var result = new List<RegistrationRequestVariation>();
                var parameters = new DynamicParameters();
                parameters.Add("RegistrationRequestId", registrationRequest.RegistrationRequestId);
                var query = "select t1.* from RegistrationRequestVariation t1 Where t1.RegistrationRequestId=@RegistrationRequestId;";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    result = results.ReadAsync<RegistrationRequestVariation>().Result.ToList();
                    var querys = string.Empty;
                    if (result.Count > 0)
                    {
                        result.ForEach(x =>
                        {
                            querys += "Delete from RegistrationRequestVariationForm where RegistrationRequestVariationId=" + x?.RegistrationRequestVariationId + ";\r\n";
                            querys += "Delete from RegistrationRequestVariation where RegistrationRequestVariationId=" + x?.RegistrationRequestVariationId + ";\r\n";
                        });
                    }
                    if (!string.IsNullOrEmpty(querys))
                    {
                        var rowsAffected = await connection.ExecuteAsync(querys);
                    }
                    List<RegistrationRequestVariation> resultData = new List<RegistrationRequestVariation>();
                    if (registrationRequest.VariationNoIds != null && registrationRequest.VariationNoIds.Count() > 0)
                    {
                        foreach (var s in registrationRequest.VariationNoIds)
                        {
                            var queryss = string.Empty;
                            queryss += "INSERT INTO RegistrationRequestVariation(RegistrationRequestID,DynamicFormDataID) OUTPUT INSERTED.RegistrationRequestVariationId VALUES " +
                        "(" + registrationRequest.RegistrationRequestId + "," + s + ");\n\r";
                            var rowsAffected1 = await connection.QuerySingleOrDefaultAsync<long>(queryss);
                            RegistrationRequestVariation registrationRequestVariation = new RegistrationRequestVariation();
                            registrationRequestVariation.DynamicFormDataId = s;
                            registrationRequestVariation.RegistrationRequestVariationId = rowsAffected1;
                            resultData.Add(registrationRequestVariation);
                        }
                    }
                    if (resultData.Count > 0)
                    {
                        var RegistrationRequestVariationIds = resultData.Select(s => s.RegistrationRequestVariationId).ToList();
                        RegistrationRequestVariationIds = RegistrationRequestVariationIds.Count() > 0 ? RegistrationRequestVariationIds : new List<long>() { -1 };
                        var queryForm = "select t1.* from RegistrationRequestVariationForm t1 JOIN RegistrationRequestVariation t2 ON t1.RegistrationRequestVariationID=t2.RegistrationRequestVariationID Where t2.RegistrationRequestID=" + registrationRequest.RegistrationRequestId + " AND t1.RegistrationRequestVariationId IN(" + string.Join(',', RegistrationRequestVariationIds) + ");";
                        var RegistrationRequestVariationForm = (await connection.QueryAsync<RegistrationRequestVariationForm>(queryForm)).ToList();
                        var queryData = string.Empty;
                        if (registrationRequest.ObjectData != null)
                        {
                            string json = JsonConvert.SerializeObject(registrationRequest.ObjectData);

                            ExpandoObject eo = JsonConvert.DeserializeObject<ExpandoObject>(json);
                            foreach (var v in eo)
                            {
                                var v1 = v.Key;
                                var v2 = (string?)v.Value;
                                if (!string.IsNullOrEmpty(v2))
                                {
                                    var v3 = v1.Split('_');
                                    var parameterss = new DynamicParameters();
                                    var DynamicFormDataIds = Convert.ToInt64(v3[0]);
                                    var vid = resultData.FirstOrDefault(f => f.DynamicFormDataId == DynamicFormDataIds);
                                    if (vid != null)
                                    {
                                        var DynamicFormDataIdData = Convert.ToInt64(v3[1]);
                                        var types = v3[2] == "Id" ? "DetailInfo" : "DetailRequirement";
                                        parameterss.Add("RegistrationRequestVariationId", vid.RegistrationRequestVariationId);
                                        parameterss.Add("DynamicFormDataID", v3[1]);
                                        parameterss.Add("Description", v2, DbType.String);
                                        parameterss.Add("DetailType", types, DbType.String);
                                        var exits = RegistrationRequestVariationForm.FirstOrDefault(f => f.DetailType == types && f.RegistrationRequestVariationId == vid.RegistrationRequestVariationId && f.DynamicFormDataId == DynamicFormDataIdData);
                                        if (exits == null)
                                        {
                                            var querya = "INSERT INTO RegistrationRequestVariationForm(DetailType,RegistrationRequestVariationId,DynamicFormDataID,Description) OUTPUT INSERTED.RegistrationRequestVariationFormID VALUES " +
                                                "(@DetailType,@RegistrationRequestVariationId,@DynamicFormDataID,@Description)";
                                            var rowsAffecteda = await connection.QuerySingleOrDefaultAsync<long>(querya, parameterss);
                                        }
                                        else
                                        {
                                            parameterss.Add("RegistrationRequestVariationFormId", exits?.RegistrationRequestVariationFormId);
                                            var querya = "UPDATE RegistrationRequestVariationForm SET DetailType=@DetailType,Description=@Description WHERE RegistrationRequestVariationFormId = @RegistrationRequestVariationFormId";
                                            await connection.ExecuteAsync(querya, parameterss);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return result;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<RegistrationRequestDueDateAssignment> InsertorUpdateRegistrationRequestDueDateAssignment(RegistrationRequestDueDateAssignment value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("RegistrationRequestDueDateAssignmentId", value.RegistrationRequestDueDateAssignmentId);
                    parameters.Add("Description", value.Description, DbType.String);
                    parameters.Add("SessionId", value.SessionId, DbType.Guid);
                    parameters.Add("DueDate", value.DueDate, DbType.DateTime);
                    parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                    parameters.Add("AddedByUserId", value.AddedByUserId);
                    parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                    parameters.Add("ModifiedUserId", value.ModifiedUserId);
                    parameters.Add("DepartmentId", value.DepartmentId);
                    parameters.Add("StatusCodeId", value.StatusCodeId);
                    parameters.Add("RegistrationRequestId", value.RegistrationRequestId);
                    if (value.RegistrationRequestDueDateAssignmentId > 0)
                    {
                        var query = "UPDATE RegistrationRequestDueDateAssignment SET RegistrationRequestId=@RegistrationRequestId,Description=@Description,DepartmentId=@DepartmentId,DueDate=@DueDate,SessionId =@SessionId,ModifiedUserId=@ModifiedUserId," +
                            "ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID WHERE RegistrationRequestDueDateAssignmentId = @RegistrationRequestDueDateAssignmentId";

                        await connection.ExecuteAsync(query, parameters);
                    }
                    else
                    {
                        var query = "INSERT INTO RegistrationRequestDueDateAssignment(RegistrationRequestId,Description,DepartmentId,DueDate,SessionId,AddedByUserId,AddedDate,ModifiedUserId,ModifiedDate,StatusCodeId) OUTPUT INSERTED.RegistrationRequestDueDateAssignmentId VALUES " +
                            "(@RegistrationRequestId,@Description,@DepartmentId,@DueDate,@SessionId,@AddedByUserId,@AddedDate,@ModifiedUserId,@ModifiedDate,@StatusCodeId)";

                        var rowsAffected = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        value.RegistrationRequestDueDateAssignmentId = rowsAffected;
                    }
                    return value;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }


        public async Task<IReadOnlyList<RegistrationRequestAssignmentOfJob>> GetRegistrationRequestAssignmentOfJob(long? RegistrationRequestId, long? DepartmentId)
        {
            try
            {
                var result = new List<RegistrationRequestAssignmentOfJob>();
                var parameters = new DynamicParameters();
                parameters.Add("RegistrationRequestId", RegistrationRequestId);
                parameters.Add("DepartmentId", DepartmentId);
                var query = "select tt3.RegistrationRequestDepartmentId,tt3.DepartmentUniqueSessionId,t1.*,t3.UserName as AddedBy,t4.UserName as ModifiedBy,tt2.Name as DepartmentName from RegistrationRequestAssignmentOfJob t1\r\n" +
                    "JOIN RegistrationRequestDepartment tt3 ON t1.RegistrationRequestDepartmentId=tt3.RegistrationRequestDepartmentId JOIN RegistrationRequest t2 ON t1.RegistrationRequestID=t2.RegistrationRequestID\r\n" +
                    "LEFT JOIN Department tt2 ON t1.DepartmentID=tt2.DepartmentID\r\nJOIN ApplicationUser t3 ON t3.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t4 ON t4.UserID=t1.ModifiedUserID Where   t1.DepartmentId=@DepartmentId AND t1.RegistrationRequestId=@RegistrationRequestId;";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    result = results.ReadAsync<RegistrationRequestAssignmentOfJob>().Result.ToList();
                }
                return result;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<RegistrationRequestAssignmentOfJob> InsertorUpdateRegistrationRequestAssignmentOfJob(RegistrationRequestAssignmentOfJob value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("RegistrationRequestAssignmentOfJobId", value.RegistrationRequestAssignmentOfJobId);
                    parameters.Add("DetailInforamtionByGuideline", value.DetailInforamtionByGuideline, DbType.String);
                    parameters.Add("DetailRequirement", value.DetailRequirement, DbType.String);
                    parameters.Add("SessionId", value.SessionId, DbType.Guid);
                    parameters.Add("TargetDate", value.TargetDate, DbType.DateTime);
                    parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                    parameters.Add("AddedByUserId", value.AddedByUserId);
                    parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                    parameters.Add("ModifiedUserId", value.ModifiedUserId);
                    parameters.Add("DepartmentId", value.DepartmentId);
                    parameters.Add("StatusCodeId", value.StatusCodeId);
                    parameters.Add("RegistrationRequestId", value.RegistrationRequestId);
                    parameters.Add("JobNo", value.JobNo, DbType.String);
                    parameters.Add("DynamicFormDataId", value.DynamicFormDataId);
                    if (value.RegistrationRequestAssignmentOfJobId > 0)
                    {
                        var query = "UPDATE RegistrationRequestAssignmentOfJob SET DynamicFormDataId=@DynamicFormDataId,JobNo=@JobNo,DetailRequirement=@DetailRequirement,RegistrationRequestId=@RegistrationRequestId,DetailInforamtionByGuideline=@DetailInforamtionByGuideline,DepartmentId=@DepartmentId,TargetDate=@TargetDate,SessionId =@SessionId,ModifiedUserId=@ModifiedUserId," +
                            "ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID WHERE RegistrationRequestAssignmentOfJobId = @RegistrationRequestAssignmentOfJobId";

                        await connection.ExecuteAsync(query, parameters);
                    }
                    else
                    {
                        var query = "INSERT INTO RegistrationRequestAssignmentOfJob(DynamicFormDataId,JobNo,DetailRequirement,RegistrationRequestId,DetailInforamtionByGuideline,DepartmentId,TargetDate,SessionId,AddedByUserId,AddedDate,ModifiedUserId,ModifiedDate,StatusCodeId) OUTPUT INSERTED.RegistrationRequestAssignmentOfJobId VALUES " +
                            "(@DynamicFormDataId,@JobNo,@DetailRequirement,@RegistrationRequestId,@DetailInforamtionByGuideline,@DepartmentId,@TargetDate,@SessionId,@AddedByUserId,@AddedDate,@ModifiedUserId,@ModifiedDate,@StatusCodeId)";

                        var rowsAffected = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        value.RegistrationRequestAssignmentOfJobId = rowsAffected;
                    }
                    return value;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<RegistrationRequestAssignmentOfJob> DeleteRegistrationRequestAssignmentOfJob(RegistrationRequestAssignmentOfJob value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("RegistrationRequestAssignmentOfJobId", value.RegistrationRequestAssignmentOfJobId);
                        var query = "Delete from RegistrationRequestAssignmentOfJob  WHERE RegistrationRequestAssignmentOfJobId = @RegistrationRequestAssignmentOfJobId";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        return value;
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


        public async Task<IReadOnlyList<RegistrationRequestProgressByRegistrationDepartment>> GetRegistrationRequestProgressByRegistrationDepartment(long? RegistrationRequestId)
        {
            try
            {
                var result = new List<RegistrationRequestProgressByRegistrationDepartment>();
                var parameters = new DynamicParameters();
                parameters.Add("RegistrationRequestId", RegistrationRequestId);
                var query = "select t1.*,t3.UserName as AddedBy,t4.UserName as ModifiedBy from RegistrationRequestProgressByRegistrationDepartment t1\r\nJOIN RegistrationRequest t2 ON t1.RegistrationRequestID=t2.RegistrationRequestID\r\nJOIN ApplicationUser t3 ON t3.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t4 ON t4.UserID=t1.ModifiedUserID Where t1.RegistrationRequestId=@RegistrationRequestId;";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    result = results.ReadAsync<RegistrationRequestProgressByRegistrationDepartment>().Result.ToList();
                }
                return result;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<RegistrationRequestProgressByRegistrationDepartment> DeleteRegistrationRequestProgressByRegistrationDepartment(RegistrationRequestProgressByRegistrationDepartment value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("RegistrationRequestProgressByRegistrationDepartmentId", value.RegistrationRequestProgressByRegistrationDepartmentId);
                        var query = "Delete from RegistrationRequestComittmentLetter  WHERE RegistrationRequestProgressByRegistrationDepartmentId = @RegistrationRequestProgressByRegistrationDepartmentId;";
                        query += "Delete from RegistrationRequestQueries  WHERE RegistrationRequestProgressByRegistrationDepartmentId = @RegistrationRequestProgressByRegistrationDepartmentId;";
                        query += "Delete from RegistrationRequestProgressByRegistrationDepartment  WHERE RegistrationRequestProgressByRegistrationDepartmentId = @RegistrationRequestProgressByRegistrationDepartmentId;";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        return value;
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
        public async Task<RegistrationRequestProgressByRegistrationDepartment> InsertorUpdateRegistrationRequestProgressByRegistrationDepartment(RegistrationRequestProgressByRegistrationDepartment value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("RegistrationRequestProgressByRegistrationDepartmentId", value.RegistrationRequestProgressByRegistrationDepartmentId);
                    parameters.Add("ExpectedSubmissionDate", value.ExpectedSubmissionDate, DbType.DateTime);
                    parameters.Add("SessionId", value.SessionId, DbType.Guid);
                    parameters.Add("ExpectedApprovalDate", value.ExpectedApprovalDate, DbType.DateTime);
                    parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                    parameters.Add("AddedByUserId", value.AddedByUserId);
                    parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                    parameters.Add("ModifiedUserId", value.ModifiedUserId);
                    parameters.Add("ApprovalDate", value.ApprovalDate, DbType.DateTime);
                    parameters.Add("CCCloseDate", value.CCCloseDate, DbType.DateTime);
                    parameters.Add("StatusCodeId", value.StatusCodeId);
                    parameters.Add("RegistrationRequestId", value.RegistrationRequestId);
                    if (value.RegistrationRequestProgressByRegistrationDepartmentId > 0)
                    {
                        var query = "UPDATE RegistrationRequestProgressByRegistrationDepartment SET CCCloseDate=@CCCloseDate,RegistrationRequestId=@RegistrationRequestId,ExpectedSubmissionDate=@ExpectedSubmissionDate,ApprovalDate=@ApprovalDate,ExpectedApprovalDate=@ExpectedApprovalDate,SessionId =@SessionId,ModifiedUserId=@ModifiedUserId," +
                            "ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID WHERE RegistrationRequestProgressByRegistrationDepartmentId = @RegistrationRequestProgressByRegistrationDepartmentId";

                        await connection.ExecuteAsync(query, parameters);
                    }
                    else
                    {
                        var query = "INSERT INTO RegistrationRequestProgressByRegistrationDepartment(CCCloseDate,RegistrationRequestId,ExpectedSubmissionDate,ApprovalDate,ExpectedApprovalDate,SessionId,AddedByUserId,AddedDate,ModifiedUserId,ModifiedDate,StatusCodeId) OUTPUT INSERTED.RegistrationRequestProgressByRegistrationDepartmentId VALUES " +
                            "(@CCCloseDate,@RegistrationRequestId,@ExpectedSubmissionDate,@ApprovalDate,@ExpectedApprovalDate,@SessionId,@AddedByUserId,@AddedDate,@ModifiedUserId,@ModifiedDate,@StatusCodeId)";

                        var rowsAffected = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        value.RegistrationRequestProgressByRegistrationDepartmentId = rowsAffected;
                    }
                    return value;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }




        public async Task<RegistrationRequestComittmentLetter> InsertorUpdateRegistrationRequestComittmentLetter(RegistrationRequestComittmentLetter value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("RegistrationRequestComittmentLetterId", value.RegistrationRequestComittmentLetterId);
                    parameters.Add("CommitmentInformation", value.CommitmentInformation, DbType.String);
                    parameters.Add("SessionId", value.SessionId, DbType.Guid);
                    parameters.Add("CommittmentDate", value.CommittmentDate, DbType.DateTime);
                    parameters.Add("CommitmentTime", value.CommitmentTime, DbType.DateTime);
                    parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                    parameters.Add("AddedByUserId", value.AddedByUserId);
                    parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                    parameters.Add("ModifiedUserId", value.ModifiedUserId);
                    parameters.Add("ActionByDeptId", value.ActionByDeptId);
                    parameters.Add("StatusCodeId", value.StatusCodeId);
                    parameters.Add("IsCommitmentDate", value.IsCommitmentDate == true ? true : null);
                    parameters.Add("RegistrationRequestProgressByRegistrationDepartmentId", value.RegistrationRequestProgressByRegistrationDepartmentId);
                    if (value.RegistrationRequestComittmentLetterId > 0)
                    {
                        var query = "UPDATE RegistrationRequestComittmentLetter SET IsCommitmentDate=@IsCommitmentDate,CommitmentTime=@CommitmentTime,RegistrationRequestProgressByRegistrationDepartmentId=@RegistrationRequestProgressByRegistrationDepartmentId,CommitmentInformation=@CommitmentInformation,ActionByDeptId=@ActionByDeptId,CommittmentDate=@CommittmentDate,SessionId =@SessionId,ModifiedUserId=@ModifiedUserId," +
                            "ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID WHERE RegistrationRequestComittmentLetterId = @RegistrationRequestComittmentLetterId";

                        await connection.ExecuteAsync(query, parameters);
                    }
                    else
                    {
                        var query = "INSERT INTO RegistrationRequestComittmentLetter(IsCommitmentDate,CommitmentTime,RegistrationRequestProgressByRegistrationDepartmentId,CommitmentInformation,ActionByDeptId,CommittmentDate,SessionId,AddedByUserId,AddedDate,ModifiedUserId,ModifiedDate,StatusCodeId) OUTPUT INSERTED.RegistrationRequestComittmentLetterId VALUES " +
                            "(@IsCommitmentDate,@CommitmentTime,@RegistrationRequestProgressByRegistrationDepartmentId,@CommitmentInformation,@ActionByDeptId,@CommittmentDate,@SessionId,@AddedByUserId,@AddedDate,@ModifiedUserId,@ModifiedDate,@StatusCodeId)";

                        var rowsAffected = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        value.RegistrationRequestComittmentLetterId = rowsAffected;
                    }
                    return value;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<IReadOnlyList<RegistrationRequestComittmentLetter>> GetRegistrationRequestComittmentLetter(long? RegistrationRequestProgressByRegistrationDepartmentId)
        {
            try
            {
                var result = new List<RegistrationRequestComittmentLetter>();
                var parameters = new DynamicParameters();
                parameters.Add("RegistrationRequestProgressByRegistrationDepartmentId", RegistrationRequestProgressByRegistrationDepartmentId);
                var query = "select t5.CodeValue As StatusCode,t1.*,t3.UserName as AddedBy,t4.UserName as ModifiedBy,tt2.Name as DepartmentName from RegistrationRequestComittmentLetter t1\r\n" +
                    "LEFT JOIN Department tt2 ON t1.ActionByDeptID=tt2.DepartmentID\r\n" +
                    "JOIN ApplicationUser t3 ON t3.UserID=t1.AddedByUserID\r\n" +
                    "LEFT JOIN ApplicationUser t4 ON t4.UserID=t1.ModifiedUserID " +
                    "LEFT JOIN CodeMaster t5 ON t5.CodeID=t1.StatusCodeID " +
                    "Where t1.RegistrationRequestProgressByRegistrationDepartmentId=@RegistrationRequestProgressByRegistrationDepartmentId;";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    result = results.ReadAsync<RegistrationRequestComittmentLetter>().Result.ToList();
                }
                return result;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<RegistrationRequestComittmentLetter> DeleteRegistrationRequestComittmentLetter(RegistrationRequestComittmentLetter value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("RegistrationRequestComittmentLetterId", value.RegistrationRequestComittmentLetterId);
                        var query = "Delete from RegistrationRequestComittmentLetter  WHERE RegistrationRequestComittmentLetterId = @RegistrationRequestComittmentLetterId";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        return value;
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

        public async Task<RegistrationRequestQueries> InsertorUpdateRegistrationRequestQueries(RegistrationRequestQueries value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("RegistrationRequestQueriesId", value.RegistrationRequestQueriesId);
                    parameters.Add("Requirement", value.Requirement, DbType.String);
                    parameters.Add("SessionId", value.SessionId, DbType.Guid);
                    parameters.Add("DateOfQueries", value.DateOfQueries, DbType.DateTime);
                    parameters.Add("DueDate", value.DueDate, DbType.DateTime);
                    parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                    parameters.Add("AddedByUserId", value.AddedByUserId);
                    parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                    parameters.Add("ModifiedUserId", value.ModifiedUserId);
                    parameters.Add("Assignment", value.Assignment, DbType.String);
                    parameters.Add("StatusCodeId", value.StatusCodeId);
                    parameters.Add("DepartmentDueDate", value.DepartmentDueDate, DbType.DateTime);
                    parameters.Add("RegistrationRequestProgressByRegistrationDepartmentId", value.RegistrationRequestProgressByRegistrationDepartmentId);
                    if (value.RegistrationRequestQueriesId > 0)
                    {
                        var query = "UPDATE RegistrationRequestQueries SET DepartmentDueDate=@DepartmentDueDate,DueDate=@DueDate,RegistrationRequestProgressByRegistrationDepartmentId=@RegistrationRequestProgressByRegistrationDepartmentId,Requirement=@Requirement,Assignment=@Assignment,DateOfQueries=@DateOfQueries,SessionId =@SessionId,ModifiedUserId=@ModifiedUserId," +
                            "ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID WHERE RegistrationRequestQueriesId = @RegistrationRequestQueriesId";

                        await connection.ExecuteAsync(query, parameters);
                    }
                    else
                    {
                        var query = "INSERT INTO RegistrationRequestQueries(DepartmentDueDate,DateOfQueries,DueDate,RegistrationRequestProgressByRegistrationDepartmentId,Requirement,Assignment,SessionId,AddedByUserId,AddedDate,ModifiedUserId,ModifiedDate,StatusCodeId) OUTPUT INSERTED.RegistrationRequestQueriesId VALUES " +
                            "(@DepartmentDueDate,@DateOfQueries,@DueDate,@RegistrationRequestProgressByRegistrationDepartmentId,@Requirement,@Assignment,@SessionId,@AddedByUserId,@AddedDate,@ModifiedUserId,@ModifiedDate,@StatusCodeId)";

                        var rowsAffected = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        value.RegistrationRequestQueriesId = rowsAffected;
                    }
                    var parameterss = new DynamicParameters();
                    parameterss.Add("RegistrationRequestQueriesId", value.RegistrationRequestQueriesId);
                    var querys = "Delete from RegistrationRequestQueriesAuthority  WHERE RegistrationRequestQueriesId = @RegistrationRequestQueriesId";
                    var rowsAffecteds = await connection.ExecuteAsync(querys, parameterss);
                    var query1 = string.Empty;
                    if (value.SelectUserIDs != null && value.SelectUserIDs.Count() > 0)
                    {
                        foreach (var item in value.SelectUserIDs)
                        {

                            query1 += "INSERT INTO [RegistrationRequestQueriesAuthority](RegistrationRequestQueriesId,UserId) OUTPUT INSERTED.RegistrationRequestQueriesAuthorityId " +
                                "VALUES (" + value.RegistrationRequestQueriesId + "," + item + ");\r\n";

                        }
                    }
                    if (!string.IsNullOrEmpty(query1))
                    {
                        await connection.QuerySingleOrDefaultAsync<long>(query1, parameters);
                    }
                    return value;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<IReadOnlyList<RegistrationRequestQueries>> GetRegistrationRequestQueries(long? RegistrationRequestProgressByRegistrationDepartmentId)
        {
            try
            {
                var result = new List<RegistrationRequestQueries>(); var resultData = new List<RegistrationRequestQueriesAuthority>();
                var parameters = new DynamicParameters();
                parameters.Add("RegistrationRequestProgressByRegistrationDepartmentId", RegistrationRequestProgressByRegistrationDepartmentId);
                var query = "select t5.CodeValue As StatusCode,t1.*,t3.UserName as AddedBy,t4.UserName as ModifiedBy from RegistrationRequestQueries t1\r\n" +
                    "JOIN ApplicationUser t3 ON t3.UserID=t1.AddedByUserID\r\n" +
                    "LEFT JOIN ApplicationUser t4 ON t4.UserID=t1.ModifiedUserID " +
                     "LEFT JOIN CodeMaster t5 ON t5.CodeID=t1.StatusCodeID " +
                    "Where t1.RegistrationRequestProgressByRegistrationDepartmentId=@RegistrationRequestProgressByRegistrationDepartmentId;";
                query += "select * from RegistrationRequestQueriesAuthority;";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    result = results.ReadAsync<RegistrationRequestQueries>().Result.ToList();
                    resultData = results.ReadAsync<RegistrationRequestQueriesAuthority>().Result.ToList();
                }
                if (result != null && result.Count() > 0)
                {
                    result.ForEach(s =>
                    {
                        s.SelectUserIDs = resultData.Where(w => w.RegistrationRequestQueriesId == s.RegistrationRequestQueriesId).Select(s => s.UserId).ToList();
                    });
                }
                return result;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<RegistrationRequestQueries> DeleteRegistrationRequestQueries(RegistrationRequestQueries value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("RegistrationRequestQueriesId", value.RegistrationRequestQueriesId);
                        var query = "Delete from RegistrationRequestQueriesAuthority  WHERE RegistrationRequestQueriesId = @RegistrationRequestQueriesId;";
                        query += "Delete from RegistrationRequestQueries  WHERE RegistrationRequestQueriesId = @RegistrationRequestQueriesId;";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        return value;
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
        public async Task<IReadOnlyList<ActivityEmailTopicsModel>> GetActivityEmailTopicList(string SessionIds)
        {

            try
            {
                var query = "select  * from ActivityEmailTopics where ActivityType='RegistrationRequest' AND SessionId in(" + SessionIds + ");";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ActivityEmailTopicsModel>(query, null)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<RegistrationRequestAssignmentOfJob> GetRegistrationRequestAssignmentOfJobBySessionIdAsync(Guid? SessionId)
        {
            try
            {
                RegistrationRequestAssignmentOfJob dynamicFormData = new RegistrationRequestAssignmentOfJob();
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                var query = "select t1.*,t2.RegistrationRequestDepartmentId,t2.DepartmentUniqueSessionId from RegistrationRequestAssignmentOfJob t1 JOIN RegistrationRequestDepartment t2 ON t1.RegistrationRequestDepartmentId=t2.RegistrationRequestDepartmentId Where t1.SessionId=@SessionId";
                using (var connection = CreateConnection())
                {
                    dynamicFormData = await connection.QueryFirstOrDefaultAsync<RegistrationRequestAssignmentOfJob>(query, parameters);
                    if (dynamicFormData != null)
                    {
                        if (dynamicFormData.DepartmentUniqueSessionId != null)
                        {
                            var _activityEmailTopics = await GetActivityEmailTopicList("'" + dynamicFormData.DepartmentUniqueSessionId.ToString() + "'");
                            var _activityEmailTopicsOne = _activityEmailTopics.FirstOrDefault(f => f.SessionId == dynamicFormData.DepartmentUniqueSessionId);
                            if (_activityEmailTopicsOne != null)
                            {
                                dynamicFormData.EmailTopicSessionId = _activityEmailTopicsOne.EmailTopicSessionId;
                                if (_activityEmailTopicsOne.EmailTopicSessionId != null)
                                {
                                    if (_activityEmailTopicsOne.IsDraft == false)
                                    {
                                        dynamicFormData.IsDraft = false;
                                    }
                                    if (_activityEmailTopicsOne.IsDraft == true)
                                    {
                                        dynamicFormData.IsDraft = true;
                                    }
                                }
                            }
                        }
                    }

                }
                return dynamicFormData;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ActivityEmailTopics> GetActivityEmailTopicsExits(Guid? SessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                var query = "select t1.ActivityEmailTopicID,\r\nt1.ActivityMasterId,\r\nt1.ManufacturingProcessId,\r\nt1.CategoryActionId,\r\nt1.ActionId,\r\nt1.Comment,\r\nt1.DocumentSessionId,\r\nt1.SubjectName,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.SessionId,\r\nt1.EmailTopicSessionId,\r\nt1.ActivityType,\r\nt1.FromId,\r\nt1.ToIds,\r\nt1.CcIds,\r\nt1.Tags,\r\nt1.BackURL,\r\nt1.IsDraft from ActivityEmailTopics t1 WHERE  t1.activityType='RegistrationRequest' AND t1.SessionId=@SessionId";

                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<ActivityEmailTopics>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<RegistrationRequestAssignmentOfJob> InsertCreateEmailRegistrationRequestAssignmentOfJob(RegistrationRequestAssignmentOfJob dynamicFormData)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        string emailContents = string.Empty;
                        var exitsData = await GetActivityEmailTopicsExits(dynamicFormData.DepartmentUniqueSessionId);
                        var aData = await GetRegistrationRequestAssignmentOfJob(dynamicFormData.RegistrationRequestId, dynamicFormData.DepartmentId);
                        if (aData != null)
                        {
                            emailContents = "<html><head><style>table {font-family: arial, sans-serif; border-collapse: collapse;width: 100%;}td, th { border: 1px solid #dddddd;text-align: left;padding: 8px;}</style></head><body><table>";
                            emailContents += "<tr><th>Department</th><th>No</th><th>Detail Inforamtion By Guideline</th><th>Detail Requirement</th><th>Target Date</th></tr>";
                            aData.ToList().ForEach(e =>
                            {
                                emailContents += "<tr><td>" + e.DepartmentName + "</td><td>" + e.JobNo + "</td><td>" + e.DetailInforamtionByGuideline + "</td><td>" + e.DetailRequirement + "</td>";
                                string startDate = string.Empty;
                                if (e.TargetDate != DateTime.MinValue)
                                {
                                    if (e.TargetDate != null)
                                    {
                                        startDate = e.TargetDate.Value.ToString("dd-MMM-yyyy");
                                    }
                                }
                                emailContents += "<td>" + startDate + "</td></tr>";
                            });
                            emailContents += "</table></body></html>";

                        }
                        var parameters = new DynamicParameters();
                        parameters.Add("activityType", "RegistrationRequest", DbType.String);
                        parameters.Add("BackUrl", dynamicFormData.BackUrl, DbType.String);
                        parameters.Add("subjectName", dynamicFormData.SubjectName, DbType.String);
                        parameters.Add("Comment", dynamicFormData.Comment, DbType.String);
                        parameters.Add("SessionId", dynamicFormData.DepartmentUniqueSessionId, DbType.Guid);
                        parameters.Add("AddedByUserID", dynamicFormData.AddedByUserId);
                        parameters.Add("ModifiedByUserID", dynamicFormData.ModifiedUserId);
                        parameters.Add("AddedDate", dynamicFormData.AddedDate, DbType.DateTime);
                        parameters.Add("ModifiedDate", dynamicFormData.ModifiedDate, DbType.DateTime);
                        parameters.Add("EmailCommentTable", emailContents, DbType.String);
                        parameters.Add("StatusCodeID", 1);
                        if (exitsData == null)
                        {
                            var query = "INSERT INTO ActivityEmailTopics(EmailCommentTable,Comment,subjectName,SessionId,AddedByUserID," +
                         "ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,activityType,BackUrl) VALUES " +
                         "(@EmailCommentTable,@Comment,@subjectName,@SessionId,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@activityType,@BackUrl)";

                            await connection.ExecuteAsync(query, parameters);

                        }
                        else
                        {
                            if (exitsData.EmailActivitySessionId == null)
                            {
                                parameters.Add("ActivityEmailTopicID", exitsData.ActivityEmailTopicID);
                                var querya = "UPDATE ActivityEmailTopics SET Comment=@Comment,activityType=@activityType,subjectName=@subjectName,ModifiedDate=@ModifiedDate,ModifiedByUserID=@ModifiedByUserID,EmailCommentTable=@EmailCommentTable WHERE ActivityEmailTopicID = @ActivityEmailTopicID";
                                await connection.ExecuteAsync(querya, parameters);
                            }
                        }

                        return dynamicFormData;
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

        public async Task<RegistrationRequestDepartmentEmailCreate> InsertCreateEmailRegistrationRequestAssignmentOfJobSubjectWise(RegistrationRequestDepartmentEmailCreate valueData)
        {


            //string strJson = JsonConvert.SerializeObject(valueData);
            //string strJson = JToken.Parse("{\"From\":\"CRTDEV\",\"FromId\":1,\"MainSubjectName\":\"Reg PA12\",\"ToIds\":[4,2,15],\"CCIds\":[81,3,17,84],\"RegistrationRequestAssignmentOfJobs\":[{\"RegistrationRequestAssignmentOfJobId\":99,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":\"2025-04-01T00:00:00\",\"JobNo\":\"A10. Contraindications\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"A1\",\"DetailRequirement\":\"NA\",\"SessionId\":\"f422fce9-e62d-4829-8e10-8588537096a3\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45181,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":100,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":\"2025-03-31T00:00:00\",\"JobNo\":\"A11. Warnings and Precautions\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"A2\",\"DetailRequirement\":\"Na\",\"SessionId\":\"a875c705-8c06-42bd-aaef-3833c7535835\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45182,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":101,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"A12. Interactions with Other Medicaments\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"A3\",\"DetailRequirement\":\"Na\",\"SessionId\":\"5bbe3b31-1102-4f9f-b1c9-4669e3b7b97d\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45183,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":102,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"A13. Pregnancy and Lactation\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"A43dd\",\"DetailRequirement\":\"NA\",\"SessionId\":\"cba2f07d-1c27-4bf2-acb1-30d3d05ae46d\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45184,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":105,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"A14. Side Effects\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"A67\",\"DetailRequirement\":\"NA\",\"SessionId\":\"397cb808-81e4-46a8-b80a-0c79dd6a7b2b\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45185,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":116,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"A15. Symptoms and Treatment of Overdose\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"NA\",\"DetailRequirement\":\"NA\",\"SessionId\":\"3a277530-7f4a-4776-ad2b-c4a816a2f2fe\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45186,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":117,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"A16. Effects on Ability to Drive and Use Machine\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"NA\",\"DetailRequirement\":\"NA\",\"SessionId\":\"ab71eab8-7356-42f3-b843-b53e2373361a\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45187,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":118,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"A17. Preclinical Safety Data\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"NA\",\"DetailRequirement\":\"NA\",\"SessionId\":\"44369b0e-35a9-4472-ad2d-c0c0860bc52b\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45188,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":119,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"A18. Instructions for Use\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"NA\",\"DetailRequirement\":\"NA\",\"SessionId\":\"988e90e5-7e4a-4247-bcb4-c168f39becf6\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45189,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":120,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"A19. Storage Conditions\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"NA\",\"DetailRequirement\":\"NA\",\"SessionId\":\"0c3e39af-5f5c-4e43-9976-173feda48ffc\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45190,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":121,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"A20. Shelf Life\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"NA\",\"DetailRequirement\":\"NA\",\"SessionId\":\"c5353d10-d422-42e2-b9eb-0c6ce25e37a3\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45191,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":122,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"A21. ATC Code\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"NA\",\"DetailRequirement\":\"NA\",\"SessionId\":\"b3284d89-7d4d-40ab-bbfd-be058aa89c42\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45192,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":123,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"A4. Product Description\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"NA\",\"DetailRequirement\":\"NA\",\"SessionId\":\"03dc7437-1d7c-44db-b156-f5381fc510d5\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45194,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":124,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"A5. Pharmacodynamics\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"NA\",\"DetailRequirement\":\"NA\",\"SessionId\":\"aca7e247-e419-41dc-89ec-9f10a4912470\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45195,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":125,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"A6. Pharmacokinetics\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"NA\",\"DetailRequirement\":\"NA\",\"SessionId\":\"def33027-d07d-4199-af31-27fa6faad8af\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45196,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":126,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"A7. Indication\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"NA\",\"DetailRequirement\":\"NA\",\"SessionId\":\"d9b92ee9-f819-40a4-8522-b083ef7c8978\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45197,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":127,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"A8. Recommended Dose\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"NA\",\"DetailRequirement\":\"NA\",\"SessionId\":\"4d10f201-aff4-4025-a161-ab8960cae035\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45198,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":128,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"A9. Route of Administration\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"NA\",\"DetailRequirement\":\"NA\",\"SessionId\":\"32cc060d-e0c4-439d-a36b-0dc3301f671a\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45199,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":129,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"D1. Label (mock-up) for Immediate Container\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"NA\",\"DetailRequirement\":\"NA\",\"SessionId\":\"c946146b-b4f8-4f5d-b2bb-57f13878a7b9\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45201,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":130,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"D2. Label (mock-up) for Outer Carton\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"NA\",\"DetailRequirement\":\"NA\",\"SessionId\":\"ec55a442-f0c2-4bfa-8d54-0083cb3f51cf\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45202,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":131,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"D3. Proposed Package Insert\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"NA\",\"DetailRequirement\":\"NA\",\"SessionId\":\"40446bca-ad7a-4e02-953c-c7059f03c49b\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45203,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":132,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"D4. Patient Information Leaflet (PIL) / Risalah Maklumat Ubat Pesakit (RiMUP)\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"NA\",\"DetailRequirement\":\"NA\",\"SessionId\":\"baed93f7-1193-4bcb-90b1-2567eb1b1471\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45204,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":133,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"D5. Label (Mock-up) for Diluent\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"NA\",\"DetailRequirement\":\"NA\",\"SessionId\":\"35a624e9-2a88-4031-a269-1d8d5f533df6\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45206,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":134,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"E10. Summary of Product Characteristics / Product Data Sheet (if applicable)\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"NA\",\"DetailRequirement\":\"NA\",\"SessionId\":\"e5a04ada-3741-48ff-ac9f-68c8addfe5ec\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45207,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":135,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"E11. Company Core Data Sheet (CCDS) (if applicable)\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"NA\",\"DetailRequirement\":\"NA\",\"SessionId\":\"519f3da9-9a5d-42b2-87b3-3f092e2a143a\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45208,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":136,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"E14. Other Supporting Documents\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"NA\",\"DetailRequirement\":\"NA\",\"SessionId\":\"34dd4406-4f64-46d2-8494-4cea0cb77bba\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45209,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":137,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"E15. Worldwide Registration Status\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"NA\",\"DetailRequirement\":\"NA\",\"SessionId\":\"ec6ea065-7888-4732-81e9-6520df715d07\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45210,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":139,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"1.1 Brand Name\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"NA\",\"DetailRequirement\":\"NA\",\"SessionId\":\"b0a37e13-34eb-4fa8-ab22-40de7f6a58dd\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45255,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":140,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"1.2 Generic Name\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"NA\",\"DetailRequirement\":\"NA\",\"SessionId\":\"2543d0da-e34c-4653-bfd4-67b5be8a9e67\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45256,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null},{\"RegistrationRequestAssignmentOfJobId\":141,\"RegistrationRequestId\":8,\"DepartmentId\":14,\"TargetDate\":null,\"JobNo\":\"1.3 Full Product Name\",\"DepartmentName\":\"Regulatory Affairs\",\"DetailInforamtionByGuideline\":\"NA\",\"DetailRequirement\":\"NA\",\"SessionId\":\"78962be3-cc42-4406-8603-d29a07dbcd05\",\"AddedByUserId\":1,\"AddedDate\":\"2025-03-19T13:48:17.6\",\"StatusCodeId\":1,\"ModifiedUserId\":1,\"ModifiedDate\":\"2025-03-19T13:48:17.6\",\"AddedBy\":\"CRTDEV\",\"ModifiedBy\":\"CRTDEV\",\"DynamicFormDataId\":45257,\"BackUrl\":null,\"EmailTopicSessionId\":null,\"IsDraft\":null,\"SubjectName\":null,\"Comment\":null,\"RegistrationRequestDepartmentId\":1,\"DepartmentUniqueSessionId\":\"6a85290f-b2e9-4ee8-b4b2-7859b252b491\",\"IsEmailCreateDone\":true,\"EmailCreateSessionId\":null}],\"ToUserGroupIds\":null,\"CCUserGroupIds\":null,\"EmailCreateSessionId\":\"7d37e7bc-936c-4126-9b1b-e3d85b11df3c\",\"BackUrl\":\"registrationrequestlist/registrationrequestform/0c06ee90-05f7-458f-8451-e92eb4f4c2f8/0c06ee90-05f7-458f-8451-e92eb4f4c2f8\"}").ToString();
            //var strJsonData = JsonConvert.DeserializeObject<RegistrationRequestDepartmentEmailCreate>(strJson);
            //valueData = strJsonData;

            //var replyId = await _emailConversationsQueryRepository.GetConversationTopicIdList(validation.Value);

            var validation = await _emailTopicsQueryRepository.CheckEmailSubjectAsync(valueData.MainSubjectName);

            if (validation?.ID > 0)
            {
                await InsertEmailReplyRegistrationRequest(valueData, validation.ID);
            }
            else
            {
                await InsertActivityEmailTopics(valueData);
                await InsertFromRegistrationRequest(valueData);
                var tid = await _emailTopicsQueryRepository.CheckEmailSubjectAsync(valueData.MainSubjectName);
                await InsertEmailReplyRegistrationRequest(valueData, tid.ID);

            }


            var _emailTopics = await _emailTopicsQueryRepository.CheckEmailSubjectAsync(valueData.MainSubjectName);
            valueData.EmailCreateSessionId = _emailTopics.SessionId;

            //try
            //{
            //    using (var connection = CreateConnection())
            //    {
            //        try
            //        {
            //            if (valueData.RegistrationRequestAssignmentOfJobs != null && valueData.RegistrationRequestAssignmentOfJobs.Count() > 0)
            //            {
            //                foreach (var item in valueData.RegistrationRequestAssignmentOfJobs)
            //                {
            //                    string SubSubjectName = item.DepartmentName + "-" + item.JobNo;
            //                    string SubemailContents = string.Empty;
            //                    SubemailContents = "<html><head><style>table {font-family: arial, sans-serif; border-collapse: collapse;width: 100%;}td, th { border: 1px solid #dddddd;text-align: left;padding: 8px;}</style></head><body><table>";
            //                    SubemailContents += "<tr><th>Department</th><th>No</th><th>Detail Inforamtion By Guideline</th><th>Detail Requirement</th><th>Target Date</th></tr>";
            //                    SubemailContents += "<tr><td>" + item.DepartmentName + "</td><td>" + item.JobNo + "</td><td>" + item.DetailInforamtionByGuideline + "</td><td>" + item.DetailRequirement + "</td>";
            //                    string startDate = string.Empty;
            //                    if (item.TargetDate != DateTime.MinValue)
            //                    {
            //                        if (item.TargetDate != null)
            //                        {
            //                            startDate = item.TargetDate.Value.ToString("dd-MMM-yyyy");
            //                        }
            //                    }
            //                    SubemailContents += "<td>" + startDate + "</td></tr>";

            //                    SubemailContents += "</table></body></html>";


            //                    var parameters = new DynamicParameters();
            //                    parameters.Add("EmailCreateSessionId", valueData.EmailCreateSessionId, DbType.Guid);
            //                    parameters.Add("IsEmailCreateDone", 1);
            //                    parameters.Add("ModifiedDate", DateTime.Now, DbType.DateTime);
            //                    parameters.Add("ModifiedUserId", valueData.FromId);
            //                    parameters.Add("RegistrationRequestAssignmentOfJobId", item.RegistrationRequestAssignmentOfJobId);
            //                    //var querya = "UPDATE RegistrationRequestAssignmentOfJob SET ModifiedDate=@ModifiedDate,ModifiedUserId=@ModifiedUserId,IsEmailCreateDone=@IsEmailCreateDone,EmailCreateSessionId=@EmailCreateSessionId WHERE RegistrationRequestAssignmentOfJobId = @RegistrationRequestAssignmentOfJobId";
            //                    //await connection.ExecuteAsync(querya, parameters);
            //                }
            //            }

            //            return valueData;
            //        }


            //        catch (Exception exp)
            //        {
            //            throw new Exception(exp.Message, exp);
            //        }

            //    }


            //}
            //catch (Exception exp)
            //{
            //    throw new NotImplementedException();
            //}


            return valueData;
        }
        public async Task<long> InsertEmailReplyRegistrationRequest(RegistrationRequestDepartmentEmailCreate request, long topicId)
        {
            if (request != null)
            {
                try
                {
                    var toIds = request.ToIds != null && request.ToIds.Any() ? string.Join(",", request.ToIds) : "";
                    var ccIds = request.CCIds != null && request.CCIds.Any() ? string.Join(",", request.CCIds) : "";
                    var fromId = request.FromId?.ToString() ?? "";
                    var participantsList = new List<string>();
                    if (!string.IsNullOrWhiteSpace(toIds)) participantsList.Add(toIds);
                    if (!string.IsNullOrWhiteSpace(ccIds)) participantsList.Add(ccIds);
                    if (!string.IsNullOrWhiteSpace(fromId)) participantsList.Add(fromId);
                    var participants = participantsList.Any() ? string.Join(",", participantsList) : "";

                    foreach (var job in request.RegistrationRequestAssignmentOfJobs)
                    {

                        string SubemailContents = string.Empty;
                        if (job.Type == "ComittmentLetter")
                        {
                            SubemailContents = "<html><head><style>table {font-family: arial, sans-serif; border-collapse: collapse;width: 100%;}td, th { border: 1px solid #dddddd;text-align: left;padding: 8px;}</style></head><body><table>";
                            SubemailContents += "<tr><th>Department</th><th>No</th><th>Committment Date</th><th>Commitment Information</th><th>Commitment Time</th><th>Status</th></tr>";
                            SubemailContents += "<tr><td>" + job.DepartmentName + "</td>";
                            string startDate1 = string.Empty;
                            if (job.CommitmentTime != DateTime.MinValue)
                            {
                                if (job.CommittmentDate != null)
                                {
                                    startDate1 = job.CommittmentDate.Value.ToString("dd-MMM-yyyy");
                                }
                            }
                            SubemailContents += "<td>" + startDate1 + "</td>";
                            SubemailContents += "<td>" + job.CommitmentInformation + "</td>";
                            string startDate = string.Empty;
                            if (job.CommitmentTime != DateTime.MinValue)
                            {
                                if (job.CommitmentTime != null)
                                {
                                    startDate = job.CommitmentTime.Value.ToString("dd-MMM-yyyy");
                                }
                            }
                            SubemailContents += "<td>" + startDate + "</td>";
                            SubemailContents += "<td>" + job.CommitmentStatus + "</td>";


                            SubemailContents += "</tr></table></body></html>";
                        }
                        else if (job.Type == "AuthorityStatusForQuery")
                        {
                            SubemailContents = "<html><head><style>table {font-family: arial, sans-serif; border-collapse: collapse;width: 100%;}td, th { border: 1px solid #dddddd;text-align: left;padding: 8px;}</style></head><body><table>";
                            SubemailContents += "<tr><th>Date Of Queries</th><th>No</th><th>Requirement</th><th>Assignment</th><th>Authority Due Date</th><th>Department DueDate</th><th>Status</th></tr>";
                            SubemailContents += "<tr>";
                            string startDate1 = string.Empty;
                            if (job.DateOfQueries != DateTime.MinValue)
                            {
                                if (job.DateOfQueries != null)
                                {
                                    startDate1 = job.DateOfQueries.Value.ToString("dd-MMM-yyyy");
                                }
                            }
                            SubemailContents += "<td>" + startDate1 + "</td>";
                            SubemailContents += "<td>" + job.Requirement + "</td>";
                            SubemailContents += "<td>" + job.Assignment + "</td>";
                            string startDate = string.Empty;
                            if (job.DueDate != DateTime.MinValue)
                            {
                                if (job.DueDate != null)
                                {
                                    startDate = job.DueDate.Value.ToString("dd-MMM-yyyy");
                                }
                            }
                            SubemailContents += "<td>" + startDate + "</td>";
                            string startDate2 = string.Empty;
                            if (job.DepartmentDueDate != DateTime.MinValue)
                            {
                                if (job.DepartmentDueDate != null)
                                {
                                    startDate = job.DepartmentDueDate.Value.ToString("dd-MMM-yyyy");
                                }
                            }
                            SubemailContents += "<td>" + startDate2 + "</td>";
                            SubemailContents += "<td>" + job.CommitmentStatus + "</td>";


                            SubemailContents += "</tr></table></body></html>";
                        }
                        else
                        {
                            SubemailContents = "<html><head><style>table {font-family: arial, sans-serif; border-collapse: collapse;width: 100%;}td, th { border: 1px solid #dddddd;text-align: left;padding: 8px;}</style></head><body><table>";
                            SubemailContents += "<tr><th>Department</th><th>No</th><th>Detail Inforamtion By Guideline</th><th>Detail Requirement</th><th>Target Date</th></tr>";
                            SubemailContents += "<tr><td>" + job.DepartmentName + "</td><td>" + job.JobNo + "</td><td>" + job.DetailInforamtionByGuideline + "</td><td>" + job.DetailRequirement + "</td>";
                            string startDate = string.Empty;
                            if (job.TargetDate != DateTime.MinValue)
                            {
                                if (job.TargetDate != null)
                                {
                                    startDate = job.TargetDate.Value.ToString("dd-MMM-yyyy");
                                }
                            }
                            SubemailContents += "<td>" + startDate + "</td></tr>";

                            SubemailContents += "</table></body></html>";

                        }
                        var emailConversation = new EmailConversations
                        {
                            TopicID = topicId,
                            AssigntoIds = toIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList(),
                            AssignccIds = ccIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList(),
                            Name = job.DepartmentName + "-" + job.JobNo,
                            FileData = Encoding.UTF8.GetBytes(SubemailContents),
                            IsAllowParticipants = true,
                            Urgent = false,
                            Follow = "No Follow Up",
                            ReplyId = 0,
                            ParticipantId = request.FromId.Value,
                            Message = SubemailContents,
                            UserType = "Users",
                            DueDate = null,
                            AllParticipantIds = participants.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(id => long.Parse(id.Trim())).ToList(),
                            StatusCodeID = 1,
                            AddedByUserID = request.FromId,
                            IsMobile = 0,
                            AddedDate = DateTime.Now,
                            SessionId = Guid.NewGuid()
                        };

                        var response = await _emailConversationsQueryRepository.Insert(emailConversation);

                        var updatereq = await _emailConversationsQueryRepository.LastUserIDUpdate(response, request.FromId.Value);

                        var ETUpdateDate = await _emailConversationsQueryRepository.LastUpdateDateEmailTopic(topicId);


                        var conversationAssignTo = new EmailConversationAssignTo();
                        conversationAssignTo.ConversationId = response;
                        conversationAssignTo.ReplyId = 0;
                        conversationAssignTo.PlistIdss = participants;
                        conversationAssignTo.AllowPlistids = participants;
                        conversationAssignTo.TopicId = topicId;
                        conversationAssignTo.StatusCodeID = 1;
                        conversationAssignTo.AddedByUserID = request.FromId;
                        conversationAssignTo.SessionId = emailConversation.SessionId;
                        conversationAssignTo.AddedDate = emailConversation.AddedDate;
                        conversationAssignTo.AssigntoIds = toIds;
                        conversationAssignTo.AssignccIds = ccIds;

                        //conversationAssignTo.ConIds = request.ConIds;
                        var reqq = await _emailConversationsQueryRepository.InsertAssignTo_sp(conversationAssignTo);

                        var plistData = participants.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(id => long.Parse(id.Trim())).ToList();
                        if (plistData.Count > 0)
                        {
                            plistData.ToList().ForEach(async a =>
                            {
                                var forumNotifications = new EmailNotifications();
                                forumNotifications.ConversationId = response;
                                forumNotifications.TopicId = topicId;
                                forumNotifications.UserId = a;
                                forumNotifications.AddedByUserID = request.FromId;
                                forumNotifications.AddedDate = DateTime.Now;
                                forumNotifications.IsRead = request.FromId == a ? true : false;
                                await _emailConversationsQueryRepository.InsertEmailNotifications(forumNotifications);
                            });
                        }


                        using (var connection = CreateConnection())
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("EmailCreateSessionId", request.EmailCreateSessionId, DbType.Guid);
                            parameters.Add("IsEmailCreateDone", 1);
                            parameters.Add("ModifiedDate", DateTime.Now, DbType.DateTime);
                            parameters.Add("ModifiedUserId", request.FromId);
                            parameters.Add("RegistrationRequestAssignmentOfJobId", job.RegistrationRequestAssignmentOfJobId);
                            var querya = "UPDATE RegistrationRequestAssignmentOfJob SET ModifiedDate=@ModifiedDate,ModifiedUserId=@ModifiedUserId,IsEmailCreateDone=@IsEmailCreateDone,EmailCreateSessionId=@EmailCreateSessionId WHERE RegistrationRequestAssignmentOfJobId = @RegistrationRequestAssignmentOfJobId";
                            if (job.Type == "ComittmentLetter")
                            {
                                querya = "UPDATE RegistrationRequestComittmentLetter SET ModifiedDate=@ModifiedDate,ModifiedUserId=@ModifiedUserId,IsEmailCreate=@IsEmailCreateDone WHERE RegistrationRequestComittmentLetterId = @RegistrationRequestAssignmentOfJobId";
                            }
                            if (job.Type == "AuthorityStatusForQuery")
                            {
                                querya = "UPDATE RegistrationRequestQueries SET ModifiedDate=@ModifiedDate,ModifiedUserId=@ModifiedUserId,IsEmailCreate=@IsEmailCreateDone WHERE RegistrationRequestQueriesId = @RegistrationRequestAssignmentOfJobId";

                            }
                            await connection.ExecuteAsync(querya, parameters);
                        }
                    }

                    return 1;
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }

            return -1;


        }


        public async Task<long> InsertFromRegistrationRequest(RegistrationRequestDepartmentEmailCreate request)
        {
            string text = "This is a sample text!";
            byte[] fileContents = Encoding.UTF8.GetBytes(text);

            var toIds = request.ToIds != null && request.ToIds.Any() ? string.Join(",", request.ToIds) : "";
            var ccIds = request.CCIds != null && request.CCIds.Any() ? string.Join(",", request.CCIds) : "";
            var fromId = request.FromId != null ? request.FromId.ToString() : "";

            var participantsList = new List<string>();
            if (!string.IsNullOrWhiteSpace(toIds)) participantsList.Add(toIds);
            if (!string.IsNullOrWhiteSpace(ccIds)) participantsList.Add(ccIds);
            if (!string.IsNullOrWhiteSpace(fromId)) participantsList.Add(fromId);

            var participants = participantsList.Any() ? string.Join(",", participantsList) : "";


            var emailTopics = new EmailTopics
            {
                TypeId = 0,
                TicketNo = "",
                TopicName = request.MainSubjectName ?? "Default Topic",
                StartDate = DateTime.Now,
                Description = "Default Description",
                AddedByUserID = request.FromId ?? 0,
                AddedDate = DateTime.Now,
                StatusCodeID = 1,
                SessionId = request.EmailCreateSessionId,
                OnDraft = 0,
                IsAllowParticipants = true,
                NotifyUser = false,
                TagLock = false,
                Follow = "No Follow Up",
                FileData = Encoding.UTF8.GetBytes(request.MainSubjectName),
                OnBehalf = null,
                Urgent = false,
                OverDue = false,
                DueDate = null,
                UserType = "Users",
                Participants = !string.IsNullOrEmpty(participants) ? participants : null,
                isValidateSession = false,
                ActivityType = "Email",
                To = !string.IsNullOrEmpty(toIds) ? toIds : null,
                CC = !string.IsNullOrEmpty(ccIds) ? ccIds : null,
                ActionTagIds = new List<long?>(),
                UserTagIds = new List<long?>(),
                ActivityEmailTopicId = 0,
                ToUserGroup = "-1",
                CCUserGroup = request.CCUserGroupIds != null && request.CCUserGroupIds.Any()
                  ? string.Join(",", request.CCUserGroupIds)
                  : "",
            };

            var result = _emailTopicsQueryRepository.Insert(emailTopics);
            return result;



        }
        public async Task<long> InsertActivityEmailTopics(RegistrationRequestDepartmentEmailCreate request)
        {
            var activityEmailTopics = new ActivityEmailTopicsModel();
            activityEmailTopics.BackURL = request.BackUrl;
            activityEmailTopics.ActivityType = "RegistrationRequest";
            activityEmailTopics.EmailTopicSessionId = request.EmailCreateSessionId;
            activityEmailTopics.SubjectName = request.MainSubjectName.Length > 20 ? request.MainSubjectName.Substring(0, 20) : request.MainSubjectName;
            activityEmailTopics.AddedByUserID = request.FromId;
            activityEmailTopics.AddedDate = DateTime.Now;
            activityEmailTopics.IsDraft = true;
            activityEmailTopics.SessionId = Guid.NewGuid();

            var result = await _productionActivityAppQueryRepository.InserProductionActivityEmail(activityEmailTopics);

            return result.ActivityEmailTopicID;
        }
    }
}
