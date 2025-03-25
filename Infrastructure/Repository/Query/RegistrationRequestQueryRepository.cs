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

namespace Infrastructure.Repository.Query
{
    public class RegistrationRequestQueryRepository : DbConnector, IRegistrationRequestQueryRepository
    {
        private readonly IGenerateDocumentNoSeriesSeviceQueryRepository _generateDocumentNoSeriesSeviceQueryRepository;
        public RegistrationRequestQueryRepository(IConfiguration configuration, IGenerateDocumentNoSeriesSeviceQueryRepository generateDocumentNoSeriesSeviceQueryRepository)
            : base(configuration)
        {
            _generateDocumentNoSeriesSeviceQueryRepository = generateDocumentNoSeriesSeviceQueryRepository;
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
                        var query = "UPDATE RegistrationRequest SET ProfileNo=@ProfileNo,ProfileId=@ProfileId,PurposeOfRegistration=@PurposeOfRegistration,SubmissionNo=@SubmissionNo,RegistrationCountryId=@RegistrationCountryId,CCNo = @CCNo,ProductSpecificationDynamicFormId =@ProductSpecificationDynamicFormId,SessionId =@SessionId,ModifiedUserId=@ModifiedUserId," +
                            "ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID,ExpectedSubmissionDate=@ExpectedSubmissionDate WHERE RegistrationRequestId = @RegistrationRequestId";

                        await connection.ExecuteAsync(query, parameters);
                    }
                    else
                    {
                        var ProfileNo = await _generateDocumentNoSeriesSeviceQueryRepository.GenerateDocumentProfileAutoNumber(new DocumentNoSeriesModel { ProfileID = value.ProfileId, Title = "Registration Request", StatusCodeID = 710 });
                        value.ProfileNo = ProfileNo;
                        parameters.Add("ProfileNo", ProfileNo, DbType.String);
                        var query = "INSERT INTO RegistrationRequest(ProfileId,ProfileNo,PurposeOfRegistration,SubmissionNo,RegistrationCountryId,CCNo,ProductSpecificationDynamicFormId,SessionId,AddedByUserId,AddedDate,ModifiedUserId,ModifiedDate,SubmissionDate,StatusCodeId,ExpectedSubmissionDate) OUTPUT INSERTED.RegistrationRequestId VALUES " +
                            "(@ProfileId,@ProfileNo,@PurposeOfRegistration,@SubmissionNo,@RegistrationCountryId,@CCNo,@ProductSpecificationDynamicFormId,@SessionId,@AddedByUserId,@AddedDate,@ModifiedUserId,@ModifiedDate,@SubmissionDate,@StatusCodeId,@ExpectedSubmissionDate)";

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
                                        parameters.Add("RegistrationRequestAssignmentOfJobId", exitsData.RegistrationRequestAssignmentOfJobId);
                                        var query1 = "UPDATE RegistrationRequestAssignmentOfJob SET RegistrationRequestDepartmentId=@RegistrationRequestDepartmentId,DynamicFormDataId=@DynamicFormDataId,RegistrationRequestId=@RegistrationRequestId,JobNo=@JobNo,DetailRequirement=@DetailRequirement,DetailInforamtionByGuideline=@DetailInforamtionByGuideline,DepartmentId=@DepartmentId," +
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
                    parameters.Add("RegistrationRequestProgressByRegistrationDepartmentId", value.RegistrationRequestProgressByRegistrationDepartmentId);
                    if (value.RegistrationRequestComittmentLetterId > 0)
                    {
                        var query = "UPDATE RegistrationRequestComittmentLetter SET CommitmentTime=@CommitmentTime,RegistrationRequestProgressByRegistrationDepartmentId=@RegistrationRequestProgressByRegistrationDepartmentId,CommitmentInformation=@CommitmentInformation,ActionByDeptId=@ActionByDeptId,CommittmentDate=@CommittmentDate,SessionId =@SessionId,ModifiedUserId=@ModifiedUserId," +
                            "ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID WHERE RegistrationRequestComittmentLetterId = @RegistrationRequestComittmentLetterId";

                        await connection.ExecuteAsync(query, parameters);
                    }
                    else
                    {
                        var query = "INSERT INTO RegistrationRequestComittmentLetter(CommitmentTime,RegistrationRequestProgressByRegistrationDepartmentId,CommitmentInformation,ActionByDeptId,CommittmentDate,SessionId,AddedByUserId,AddedDate,ModifiedUserId,ModifiedDate,StatusCodeId) OUTPUT INSERTED.RegistrationRequestComittmentLetterId VALUES " +
                            "(@CommitmentTime,@RegistrationRequestProgressByRegistrationDepartmentId,@CommitmentInformation,@ActionByDeptId,@CommittmentDate,@SessionId,@AddedByUserId,@AddedDate,@ModifiedUserId,@ModifiedDate,@StatusCodeId)";

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
                var query = "select t1.*,t3.UserName as AddedBy,t4.UserName as ModifiedBy,tt2.Name as DepartmentName from RegistrationRequestComittmentLetter t1\r\nLEFT JOIN Department tt2 ON t1.ActionByDeptID=tt2.DepartmentID\r\nJOIN ApplicationUser t3 ON t3.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t4 ON t4.UserID=t1.ModifiedUserID Where t1.RegistrationRequestProgressByRegistrationDepartmentId=@RegistrationRequestProgressByRegistrationDepartmentId;";
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
                    parameters.Add("RegistrationRequestProgressByRegistrationDepartmentId", value.RegistrationRequestProgressByRegistrationDepartmentId);
                    if (value.RegistrationRequestQueriesId > 0)
                    {
                        var query = "UPDATE RegistrationRequestQueries SET DueDate=@DueDate,RegistrationRequestProgressByRegistrationDepartmentId=@RegistrationRequestProgressByRegistrationDepartmentId,Requirement=@Requirement,Assignment=@Assignment,DateOfQueries=@DateOfQueries,SessionId =@SessionId,ModifiedUserId=@ModifiedUserId," +
                            "ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID WHERE RegistrationRequestQueriesId = @RegistrationRequestQueriesId";

                        await connection.ExecuteAsync(query, parameters);
                    }
                    else
                    {
                        var query = "INSERT INTO RegistrationRequestQueries(DateOfQueries,DueDate,RegistrationRequestProgressByRegistrationDepartmentId,Requirement,Assignment,SessionId,AddedByUserId,AddedDate,ModifiedUserId,ModifiedDate,StatusCodeId) OUTPUT INSERTED.RegistrationRequestQueriesId VALUES " +
                            "(@DateOfQueries,@DueDate,@RegistrationRequestProgressByRegistrationDepartmentId,@Requirement,@Assignment,@SessionId,@AddedByUserId,@AddedDate,@ModifiedUserId,@ModifiedDate,@StatusCodeId)";

                        var rowsAffected = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        value.RegistrationRequestQueriesId = rowsAffected;
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
                var result = new List<RegistrationRequestQueries>();
                var parameters = new DynamicParameters();
                parameters.Add("RegistrationRequestProgressByRegistrationDepartmentId", RegistrationRequestProgressByRegistrationDepartmentId);
                var query = "select t1.*,t3.UserName as AddedBy,t4.UserName as ModifiedBy from RegistrationRequestQueries t1\r\nJOIN ApplicationUser t3 ON t3.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t4 ON t4.UserID=t1.ModifiedUserID Where t1.RegistrationRequestProgressByRegistrationDepartmentId=@RegistrationRequestProgressByRegistrationDepartmentId;";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    result = results.ReadAsync<RegistrationRequestQueries>().Result.ToList();
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
                        var query = "Delete from RegistrationRequestQueries  WHERE RegistrationRequestQueriesId = @RegistrationRequestQueriesId";
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
                            emailContents = "<html><head><style>table {font-family: arial, sans-serif; border-collapse: collapse;width: 100%;}td, th { border: 1px solid #dddddd;text-align: left;padding: 8px;}tr:nth-child(even) {background-color: #dddddd;}</style></head><body><table>";
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
    }
}
