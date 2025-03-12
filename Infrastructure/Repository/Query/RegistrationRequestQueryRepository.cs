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
                var parameters = new DynamicParameters();
                parameters.Add("RegistrationRequestId", RegistrationRequestId);

                var query = "select t1.*,t2.DynamicFormDataID as RegDynamicFormDataID,(CONCAT(t2.DynamicFormDataID,'_',t1.DynamicFormDataID,'_Id')) as RegDynamicFormDataNameID  from RegistrationRequestVariationForm t1 JOIN RegistrationRequestVariation t2 ON t1.RegistrationRequestVariationID=t2.RegistrationRequestVariationID Where t2.RegistrationRequestId = @RegistrationRequestId";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<RegistrationRequestVariationForm>(query, parameters)).ToList();
                }
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

                    var ids = result.Select(x => x.DynamicFormDataId).ToList();
                    var excelpt = ids.Except(registrationRequest.VariationNoIds).ToList();
                    var querys = string.Empty;
                    if (excelpt.Count > 0)
                    {
                        excelpt.ForEach(x =>
                        {
                            var result1 = result.FirstOrDefault(a => a.DynamicFormDataId == x);
                            if (result1 != null)
                            {
                                querys += "Delete from RegistrationRequestVariationForm where RegistrationRequestVariationId=" + result1?.RegistrationRequestVariationId + ";\r\n";
                                querys += "Delete from RegistrationRequestVariation where RegistrationRequestVariationId=" + result1?.RegistrationRequestVariationId + ";\r\n";
                            }
                        });
                        if (!string.IsNullOrEmpty(querys))
                        {
                            var rowsAffected = await connection.ExecuteAsync(querys);
                        }
                    }
                    List<RegistrationRequestVariation> resultData = new List<RegistrationRequestVariation>();
                    if (registrationRequest.VariationNoIds != null && registrationRequest.VariationNoIds.Count() > 0)
                    {
                        foreach (var s in registrationRequest.VariationNoIds)
                        {
                            var queryss = string.Empty;
                            var result1 = result.FirstOrDefault(a => a.DynamicFormDataId == s);
                            if (result1 != null)
                            {
                                resultData.Add(result1);
                            }
                            else
                            {
                                queryss += "INSERT INTO RegistrationRequestVariation(RegistrationRequestID,DynamicFormDataID) OUTPUT INSERTED.RegistrationRequestVariationId VALUES " +
                            "(" + registrationRequest.RegistrationRequestId + "," + s + ");\n\r";
                                var rowsAffected1 = await connection.QuerySingleOrDefaultAsync<long>(queryss);
                                RegistrationRequestVariation registrationRequestVariation = new RegistrationRequestVariation();
                                registrationRequestVariation.DynamicFormDataId = s;
                                registrationRequestVariation.RegistrationRequestVariationId = rowsAffected1;
                                resultData.Add(registrationRequestVariation);
                            }
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
                                        parameterss.Add("RegistrationRequestVariationId", vid.RegistrationRequestVariationId);
                                        parameterss.Add("DynamicFormDataID", v3[1]);
                                        parameterss.Add("Description", v2, DbType.String);
                                        var exits = RegistrationRequestVariationForm.FirstOrDefault(f => f.RegistrationRequestVariationId == vid.RegistrationRequestVariationId && f.DynamicFormDataId == DynamicFormDataIdData);
                                        if (exits == null)
                                        {
                                            var querya = "INSERT INTO RegistrationRequestVariationForm(RegistrationRequestVariationId,DynamicFormDataID,Description) OUTPUT INSERTED.RegistrationRequestVariationFormID VALUES " +
                                                "(@RegistrationRequestVariationId,@DynamicFormDataID,@Description)";
                                            var rowsAffecteda = await connection.QuerySingleOrDefaultAsync<long>(querya, parameterss);
                                        }
                                        else
                                        {
                                            parameterss.Add("RegistrationRequestVariationFormId", exits?.RegistrationRequestVariationFormId);
                                            var querya = "UPDATE RegistrationRequestVariationForm SET Description=@Description WHERE RegistrationRequestVariationFormId = @RegistrationRequestVariationFormId";
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
    }
}
