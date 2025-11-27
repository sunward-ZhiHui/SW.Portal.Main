using Application.Common;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModel;
using Core.EntityModels;
using Core.Repositories.Query;
using Dapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Services.Client;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastructure.Repository.Query
{
    public class DocumentProfileNoSeriesQueryRepository : QueryRepository<DocumentProfileNoSeriesModel>, IDocumentProfileNoSeriesQueryRepository
    {
        private readonly IGenerateDocumentNoSeriesSeviceQueryRepository _generateDocumentNoSeriesSeviceQueryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public DocumentProfileNoSeriesQueryRepository(IConfiguration configuration, IGenerateDocumentNoSeriesSeviceQueryRepository generateDocumentNoSeriesSeviceQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
            : base(configuration)
        {
            _generateDocumentNoSeriesSeviceQueryRepository = generateDocumentNoSeriesSeviceQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<DocumentProfileNoSeriesModel> DeleteDocumentProfileNoSeries(DocumentProfileNoSeriesModel documentProfileNoSeriesModel, long? UserId)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var result = await GetProfileNoBySession(documentProfileNoSeriesModel.SessionId);
                        var parameters = new DynamicParameters();
                        parameters.Add("ProfileID", documentProfileNoSeriesModel.ProfileID);
                        var query = string.Empty;
                        query += "DELETE  FROM DocumentProfileNoSeries WHERE ProfileID = @ProfileID;";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        var guid = Guid.NewGuid();
                        var uid = Guid.NewGuid();
                        if (result != null)
                        {
                            List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
                            auditList.Add(new HRMasterAuditTrail { PreValue = result?.Name, ColumnName = "Name" });
                            if (!string.IsNullOrEmpty(result?.Abbreviation))
                            {
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.Abbreviation, ColumnName = "Abbreviation" });
                            }
                            if (result?.DeparmentId > 0)
                            {
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.DeparmentId?.ToString(), ColumnName = "DeparmentId" });
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.DepartmentName?.ToString(), ColumnName = "DepartmentName" });
                            }
                            if (result?.ProfileTypeId > 0)
                            {
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.ProfileTypeId?.ToString(), ColumnName = "ProfileTypeId" });
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.ProfileType?.ToString(), ColumnName = "ProfileType" });
                            }

                            if (result?.GroupId > 0)
                            {
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.GroupId?.ToString(), ColumnName = "GroupId" });
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.GroupName?.ToString(), ColumnName = "GroupName" });
                            }

                            if (!string.IsNullOrEmpty(result?.GroupAbbreviation))
                            {
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.GroupAbbreviation, ColumnName = "GroupAbbreviation" });
                            }
                            if (result?.StatusCodeID > 0)
                            {
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCodeID?.ToString(), ColumnName = "StatusCodeID" });
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCode?.ToString(), ColumnName = "StatusCode" });
                            }
                            if (!string.IsNullOrEmpty(result?.Description))
                            {
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.Description, ColumnName = "Description" });
                            }
                            if (result?.IsEnableTask == true)
                            {
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.IsEnableTask?.ToString(), ColumnName = "IsEnableTask" });
                            }
                            if (!string.IsNullOrEmpty(result?.SpecialWording))
                            {
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.SpecialWording, ColumnName = "SpecialWording" });
                            }
                            if (result?.SeperatorToUse != null)
                            {
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.SeperatorToUse?.ToString(), ColumnName = "SeperatorToUse" });
                            }
                            if (result?.NoOfDigit != null)
                            {
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.NoOfDigit?.ToString(), ColumnName = "NoOfDigit" });
                            }
                            if (result?.IncrementalNo != null)
                            {
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.IncrementalNo?.ToString(), ColumnName = "IncrementalNo" });
                            }
                            if (!string.IsNullOrEmpty(result?.StartingNo))
                            {
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.StartingNo?.ToString(), ColumnName = "StartingNo" });
                            }
                            if (result?.TranslationRequired == true)
                            {
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.TranslationRequired?.ToString(), ColumnName = "TranslationRequired" });
                            }
                            if (!string.IsNullOrEmpty(result?.Note))
                            {
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.Note?.ToString(), ColumnName = "Note" });
                            }
                            if (result?.StartWithYear == true)
                            {
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.StartWithYear?.ToString(), ColumnName = "StartWithYear" });
                            }
                            if (result?.AbbreviationRequired == true)
                            {
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.AbbreviationRequired?.ToString(), ColumnName = "AbbreviationRequired" });
                            }
                            if (result?.IsGroupAbbreviation == true)
                            {
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.IsGroupAbbreviation?.ToString(), ColumnName = "IsGroupAbbreviation" });
                            }
                            if (result?.IsCategoryAbbreviation == true)
                            {
                                auditList.Add(new HRMasterAuditTrail { PreValue = result?.IsCategoryAbbreviation?.ToString(), ColumnName = "IsCategoryAbbreviation" });
                            }
                            if (!string.IsNullOrEmpty(result?.Abbreviation1))
                            {
                                string a1 = string.Empty;
                                if (!string.IsNullOrEmpty(result.Abbreviation1))
                                {
                                    dynamic? abbreviation = JsonConvert.DeserializeObject(result.Abbreviation1);
                                    if (abbreviation != null)
                                    {
                                        foreach (var item in abbreviation)
                                        {
                                            a1 += item.Name + ",";
                                        }
                                    }
                                }
                                auditList.Add(new HRMasterAuditTrail { PreValue = a1.TrimEnd(','), ColumnName = "Abbreviation1" });
                            }
                            auditList.Add(new HRMasterAuditTrail { PreValue = result?.Name, CurrentValue = result?.Name, ColumnName = "DisplayName" });
                            auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedByUserID?.ToString(), ColumnName = "ModifiedByUserID" });
                            auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "ModifiedDate" });
                            auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedByUser?.ToString(), ColumnName = "ModifiedBy" });
                            if (auditList.Count() > 0)
                            {
                                HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                                {
                                    HRMasterAuditTrailItems = auditList,
                                    Type = "DocumentProfileNoSeries",
                                    FormType = "Delete",
                                    HRMasterSetId = result?.ProfileID,
                                    IsDeleted = true,
                                    AuditUserId = UserId,
                                };
                                await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
                            }
                        }
                        return documentProfileNoSeriesModel;
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
        public async Task<IReadOnlyList<DocumentProfileNoSeriesModel>> GetDocumentNoSeriesAsync()
        {
            try
            {
                var result = new List<DocumentProfileNoSeriesModel>();
                var query = "select ProfileID,Name from DocumentProfileNoSeries";
                using (var connection = CreateConnection())
                {
                    result = (await connection.QueryAsync<DocumentProfileNoSeriesModel>(query)).ToList();
                }
                return result;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DocumentNoSeries>> GetAllDocumentNoSeriesAsync(DocumentProfileNoSeriesModel value)
        {
            try
            {
                var result = new List<DocumentNoSeries>();
                var parameters = new DynamicParameters();
                var query = "SELECT t1.*,t2.CompanyID,t2.DeparmentID,t2.Name as ProfileName,t6.PlantCode as CompanyName,t7.Name as DepartmentName FROM DocumentNoSeries t1\r\n" +
                    "JOIN DocumentProfileNoSeries t2 ON t1.ProfileID=t2.ProfileID \r\n" +
                    "LEFT JOIN Plant t6 ON t2.CompanyID=t6.PlantID \r\n" +
                    "LEFT JOIN Department t7 ON t2.DeparmentID=t7.DepartmentID\r";
                var query1 = string.Empty;
                if (value.SelectProfileID > 0)
                {
                    query1 += "\rt1.ProfileID=" + value.SelectProfileID + " AND";
                }
                if (value.CompanyId > 0)
                {
                    query1 += "\rt2.CompanyID=" + value.CompanyId + " AND";
                }
                if (value.DepartmentId > 0)
                {
                    query1 += "\rt2.DeparmentID=" + value.DepartmentId + " AND";
                }
                if (value.FileProfileTypeId > 0)
                {
                    query1 += "\rt1.FileProfileTypeId=" + value.FileProfileTypeId + " AND";
                }
                if (!string.IsNullOrEmpty(value.SampleDocumentNo))
                {
                    query1 += "\rt1.DocumentNo='" + value.SampleDocumentNo + "' AND";
                }

                if (value.AddedDate != null)
                {
                    var from = value.AddedDate.Value.ToString("yyyy-MM-dd");
                    query1 += "\rCAST(t1.AddedDate AS Date) >='" + from + "' AND";
                }
                if (value.ModifiedDate != null)
                {
                    var to = value.ModifiedDate.Value.ToString("yyyy-MM-dd");
                    query1 += "\rCAST(t1.AddedDate AS Date)<='" + to + "' AND";
                }
                query1 = query1.Trim();
                if (query1.EndsWith("AND"))
                {
                    query1 = query1.Remove(query1.Length - 3);
                }
                if (!string.IsNullOrEmpty(query1))
                {
                    query += "Where\r" + query1;
                }
                query += "\rorder by ProfileID OFFSET (" + value.PageNumber + " - 1) * " + value.PageSize + " ROWS FETCH NEXT " + value.PageSize + " ROWS ONLY";
                var appUsers = new List<ApplicationUser>(); var Plants = new List<Plant>();
                var DocumentProfileNo = new List<DocumentProfileNoSeries>(); var Departments = new List<Department>();
                using (var connection = CreateConnection())
                {
                    result = (await connection.QueryAsync<DocumentNoSeries>(query, parameters)).ToList();
                    List<long?> userIds = new List<long?>(); List<long?> depIds = new List<long?>(); List<long?> profIds = new List<long?>(); List<long?> compIds = new List<long?>();
                    if (result != null && result.Count() > 0)
                    {
                        profIds.AddRange(result.Where(w => w.ProfileId > 0).Select(a => a.ProfileId).ToList());
                        userIds.AddRange(result.Where(w => w.AddedByUserId > 0).Select(a => a.AddedByUserId).ToList());
                        userIds.AddRange(result.Where(a => a.ModifiedByUserId > 0).Select(a => a.ModifiedByUserId).ToList());
                        userIds.AddRange(result.Where(a => a.RequestorId > 0).Select(a => a.RequestorId).ToList());
                        userIds = userIds != null && userIds.Count > 0 ? userIds : new List<long?>() { -1 };
                        profIds = profIds != null && profIds.Count > 0 ? profIds : new List<long?>() { -1 };
                        var query2 = "select CONCAT(case when t3.NickName is NULL then  t3.FirstName ELSE  t3.NickName END,' | ',t3.LastName) as UserName,t3.UserId from Employee t3 where t3.userId in(" + string.Join(',', userIds.Distinct()) + ");";
                        query2 += "select ProfileID,DeparmentID,CompanyID from DocumentProfileNoSeries where ProfileID in(" + string.Join(',', profIds.Distinct()) + ");";
                        var QuerResult = await connection.QueryMultipleAsync(query2);
                        appUsers = QuerResult.Read<ApplicationUser>().ToList();
                    }
                }
                if (result != null && result.Count() > 0)
                {
                    result.ForEach(s =>
                    {
                        s.AddedBy = appUsers.FirstOrDefault(f => f.UserID == s.AddedByUserId)?.UserName;
                        s.ModifiedBy = appUsers.FirstOrDefault(f => f.UserID == s.ModifiedByUserId)?.UserName;
                        s.RequestorName = appUsers.FirstOrDefault(f => f.UserID == s.RequestorId)?.UserName;
                    });
                }
                return result;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DocumentProfileNoSeriesModel>> GetAllAsync()
        {
            try
            {
                var result = new List<DocumentProfileNoSeriesModel>();
                var query = "select t1.*,t2.CodeValue as StatusCode,t3.UserName as AddedByUser,t4.UserName as ModifiedByUser,t5.PlantCode as CompanyName,t6.Name as DepartmentName,t7.CodeValue as ProfileType,t8.Value as GroupName,t9.Value as CategoryName  from DocumentProfileNoSeries t1\r\nLEFT JOIN CodeMaster t2 ON t2.CodeID=t1.StatusCodeID\r\nLEFT JOIN ApplicationUser t3 ON t3.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t4 ON t4.UserID=t1.ModifiedByUserID\r\nLEFT JOIN Plant t5 ON t5.PlantID=t1.CompanyID\r\nLEFT JOIN Department t6 ON t6.DepartmentID=t1.DeparmentID\r\nLEFT JOIN CodeMaster t7 ON t7.CodeID=t1.ProfileTypeID\r\nLEFT JOIN ApplicationMasterDetail t8 ON t8.ApplicationMasterDetailID=t1.GroupID\r\nLEFT JOIN ApplicationMasterDetail t9 ON t9.ApplicationMasterDetailID=t1.CategoryID";

                using (var connection = CreateConnection())
                {
                    result = (await connection.QueryAsync<DocumentProfileNoSeriesModel>(query)).ToList();
                }
                if (result != null && result.Count > 0)
                {
                    result.ForEach(s =>
                    {
                        List<long?> EstablishNoSeriesCodeIds = new List<long?>();
                        if (!string.IsNullOrEmpty(s.Abbreviation1))
                        {
                            List<NumberSeriesCodeModel>? numberSeriesCodeModels = JsonConvert.DeserializeObject<List<NumberSeriesCodeModel>>(s.Abbreviation1);

                            if (numberSeriesCodeModels != null && numberSeriesCodeModels.Count() > 0)
                            {

                                numberSeriesCodeModels.OrderBy(s => s.Index).ToList().ForEach(n =>
                                {
                                    EstablishNoSeriesCodeIds.Add(n.Id);
                                });
                            }
                        }
                        if (s.EstablishNoSeriesCodeIDs != null && s.EstablishNoSeriesCodeIDs.ToList().Count > 0)
                        {
                            s.IsNoSerieswithOrganisationinfo = true;
                        }
                        s.EstablishNoSeriesCodeIDs = EstablishNoSeriesCodeIds;
                    });
                }
                return result;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DocumentProfileNoSeriesModel> GetProfileNoBySession(Guid? SessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionID", SessionId);
                var result = new DocumentProfileNoSeriesModel();
                var query = "select t1.*,t2.CodeValue as StatusCode,t3.UserName as AddedByUser,t4.UserName as ModifiedByUser,t5.PlantCode as CompanyName,t6.Name as DepartmentName,t7.CodeValue as ProfileType,t8.Value as GroupName,t9.Value as CategoryName  from DocumentProfileNoSeries t1\r\nLEFT JOIN CodeMaster t2 ON t2.CodeID=t1.StatusCodeID\r\nLEFT JOIN ApplicationUser t3 ON t3.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t4 ON t4.UserID=t1.ModifiedByUserID\r\nLEFT JOIN Plant t5 ON t5.PlantID=t1.CompanyID\r\nLEFT JOIN Department t6 ON t6.DepartmentID=t1.DeparmentID\r\nLEFT JOIN CodeMaster t7 ON t7.CodeID=t1.ProfileTypeID\r\nLEFT JOIN ApplicationMasterDetail t8 ON t8.ApplicationMasterDetailID=t1.GroupID\r\nLEFT JOIN ApplicationMasterDetail t9 ON t9.ApplicationMasterDetailID=t1.CategoryID where t1.SessionID=@SessionID;";

                using (var connection = CreateConnection())
                {
                    result = (await connection.QueryFirstOrDefaultAsync<DocumentProfileNoSeriesModel>(query, parameters));
                }
                if (result != null && result.ProfileID > 0)
                {
                    List<long?> EstablishNoSeriesCodeIds = new List<long?>();
                    if (!string.IsNullOrEmpty(result.Abbreviation1))
                    {
                        List<NumberSeriesCodeModel>? numberSeriesCodeModels = JsonConvert.DeserializeObject<List<NumberSeriesCodeModel>>(result.Abbreviation1);

                        if (numberSeriesCodeModels != null && numberSeriesCodeModels.Count() > 0)
                        {

                            numberSeriesCodeModels.OrderBy(s => s.Index).ToList().ForEach(n =>
                            {
                                EstablishNoSeriesCodeIds.Add(n.Id);
                            });
                        }
                    }
                    if (EstablishNoSeriesCodeIds != null && EstablishNoSeriesCodeIds.ToList().Count > 0)
                    {
                        result.IsNoSerieswithOrganisationinfo = true;

                    }
                    result.EstablishNoSeriesCodeIDs = EstablishNoSeriesCodeIds != null ? EstablishNoSeriesCodeIds : new List<long?>();
                }
                return result;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long?> GetDocumentNoSeriesCount(long? id)
        {
            try
            {
                long? Counts = 0;
                if (id > 0)
                {
                    var parameters = new DynamicParameters();
                    var query = string.Empty;
                    parameters.Add("ProfileID", id);
                    query = "SELECT t1.ProfileID FROM DocumentNoSeries t1 Where  t1.ProfileID=@ProfileID;";
                    using (var connection = CreateConnection())
                    {
                        var result = (await connection.QueryAsync<DocumentProfileNoSeriesModel>(query, parameters)).ToList();
                        Counts = result != null ? result.Count() : 0;
                    }
                }
                return Counts;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public DocumentProfileNoSeriesModel GetProfileNameCheckValidation(string? value, long id)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("Name", value);
                if (id > 0)
                {
                    parameters.Add("ProfileID", id);
                    query = "SELECT * FROM DocumentProfileNoSeries t1 Where  t1.ProfileID!=@ProfileID AND t1.Name = @Name";
                }
                else
                {
                    query = "SELECT * FROM DocumentProfileNoSeries t1 Where  t1.Name = @Name";
                }
                using (var connection = CreateConnection())
                {
                    return connection.QueryFirstOrDefault<DocumentProfileNoSeriesModel>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public DocumentProfileNoSeriesModel GetAbbreviationCheckValidation(string? value, string? Abbreviation, long id)
        {
            try
            {
                DocumentProfileNoSeriesModel documentProfileNoSeriesModel = new DocumentProfileNoSeriesModel();
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(Abbreviation))
                {
                    var parameters = new DynamicParameters();
                    var query = string.Empty;
                    parameters.Add("Name", value);
                    parameters.Add("Abbreviation", Abbreviation);
                    if (id > 0)
                    {
                        parameters.Add("ProfileID", id);
                        query = "SELECT * FROM DocumentProfileNoSeries t1 Where  t1.ProfileID!=@ProfileID AND (Abbreviation=@Abbreviation OR t1.Name = @Name)";
                    }
                    else
                    {
                        query = "SELECT * FROM DocumentProfileNoSeries t1 Where (Abbreviation=@Abbreviation OR t1.Name = @Name)";
                    }
                    using (var connection = CreateConnection())
                    {
                        documentProfileNoSeriesModel = connection.QueryFirstOrDefault<DocumentProfileNoSeriesModel>(query, parameters);
                    }
                }
                return documentProfileNoSeriesModel;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        async Task<string> GenerateSampleDocumentNo(DocumentProfileNoSeriesModel documentProfile)
        {
            DocumentNoSeriesModel documentNoSeriesModel = new DocumentNoSeriesModel();
            documentNoSeriesModel.ProfileID = documentProfile.ProfileID;
            documentNoSeriesModel.PlantID = documentProfile.CompanyId;
            documentNoSeriesModel.DepartmentId = documentProfile.DeparmentId;
            documentProfile.SampleDocumentNo = await _generateDocumentNoSeriesSeviceQueryRepository.GenerateSampleDocumentNoAsync(documentNoSeriesModel);
            if (!string.IsNullOrEmpty(documentProfile.SampleDocumentNo))
            {
                try
                {
                    var parameters = new DynamicParameters();
                    var query = string.Empty;
                    parameters.Add("ProfileID", documentProfile.ProfileID);
                    parameters.Add("SampleDocumentNo", documentProfile.SampleDocumentNo, DbType.String);
                    query = "update DocumentProfileNoSeries set SampleDocumentNo=@SampleDocumentNo Where  ProfileID = @ProfileID;";

                    using (var connection = CreateConnection())
                    {
                        var result = await connection.ExecuteAsync(query, parameters);
                    }
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }
            }
            return documentProfile.SampleDocumentNo;
        }
        public async Task<DocumentProfileNoSeriesModel> InsertOrUpdateDocumentProfileNoSeries(DocumentProfileNoSeriesModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("Name", value.Name, DbType.String);
                        parameters.Add("Description", value.Description, DbType.String);
                        parameters.Add("Abbreviation", value.Abbreviation == null ? string.Empty : value.Abbreviation, DbType.String);
                        parameters.Add("Abbreviation1", value.Abbreviation1 == null ? string.Empty : value.Abbreviation1, DbType.String);
                        parameters.Add("Abbreviation2", value.Abbreviation2 == null ? string.Empty : value.Abbreviation2, DbType.String);
                        parameters.Add("AbbreviationRequired", value.AbbreviationRequired == true ? true : false);
                        parameters.Add("SessionId", value.SessionId, DbType.Guid);
                        parameters.Add("SpecialWording", value.SpecialWording == null ? string.Empty : value.SpecialWording, DbType.String);
                        parameters.Add("StartWithYear", value.StartWithYear == true ? true : false);
                        parameters.Add("TranslationRequired", value.TranslationRequired == true ? true : false);
                        parameters.Add("LastCreatedDate", value.LastCreatedDate, DbType.DateTime);
                        parameters.Add("LastNoUsed", value.LastNoUsed, DbType.String);
                        parameters.Add("Note", value.Note, DbType.String);
                        parameters.Add("CategoryAbbreviation", value.CategoryAbbreviation, DbType.String);
                        parameters.Add("CategoryId", value.CategoryId);
                        parameters.Add("IsCategoryAbbreviation", value.IsCategoryAbbreviation == true ? true : false);
                        parameters.Add("CompanyId", value.CompanyId);
                        parameters.Add("DeparmentId", value.DeparmentId);
                        parameters.Add("GroupId", value.GroupId);
                        parameters.Add("GroupAbbreviation", value.GroupAbbreviation, DbType.String);
                        parameters.Add("IsGroupAbbreviation", value.IsGroupAbbreviation == true ? true : false);
                        parameters.Add("LinkId", value.LinkId);
                        parameters.Add("ProfileTypeId", value.ProfileTypeId);
                        parameters.Add("StatusCodeID", value.StatusCodeID);
                        parameters.Add("AddedByUserID", value.AddedByUserID);
                        parameters.Add("ModifiedByUserID", value.ModifiedByUserID);
                        parameters.Add("IsEnableTask", value.IsEnableTask == true ? true : false);
                        parameters.Add("AddedDate", value.AddedDate);
                        parameters.Add("ModifiedDate", value.ModifiedDate);
                        if (value.SeperatorToUse > 0)
                        {
                            parameters.Add("NoOfDigit", value.NoOfDigit);
                            parameters.Add("IncrementalNo", value.IncrementalNo);
                            parameters.Add("StartingNo", !string.IsNullOrEmpty(value.StartingNo) ? value.StartingNo : string.Empty, DbType.String);
                            parameters.Add("SeperatorToUse", value.SeperatorToUse);
                        }
                        else
                        {
                            parameters.Add("StartingNo", string.Empty, DbType.String);
                        }
                        var query = string.Empty;
                        if (value.ProfileID > 0)
                        {
                            if (parameters is DynamicParameters subDynamic)
                            {
                                query += "UPDATE DocumentProfileNoSeries SET\r";
                                var names = string.Empty;
                                if (subDynamic.ParameterNames is not null)
                                {
                                    foreach (var keyValue in subDynamic.ParameterNames)
                                    {
                                        names += keyValue + "=";
                                        names += "@" + keyValue + ",";
                                    }
                                }
                                query += names.TrimEnd(',') + "\rwhere ProfileID = " + value.ProfileID + ";";
                            }
                        }
                        else
                        {
                            if (parameters is DynamicParameters subDynamic)
                            {
                                query += "INSERT INTO DocumentProfileNoSeries(\r";
                                var values = string.Empty;
                                var names = string.Empty;
                                if (subDynamic.ParameterNames is not null)
                                {
                                    foreach (var keyValue in subDynamic.ParameterNames)
                                    {
                                        names += keyValue + ",";
                                        values += "@" + keyValue + ",";
                                    }
                                }
                                query += names.TrimEnd(',') + ")\rOUTPUT INSERTED.ProfileID VALUES(" + values.TrimEnd(',') + ");";
                            }
                        }
                        if (!string.IsNullOrEmpty(query))
                        {
                            if (value.ProfileID > 0)
                            {
                                var result = await GetProfileNoBySession(value.SessionId);
                                await connection.ExecuteAsync(query, parameters);

                                if (result != null)
                                {
                                    List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
                                    bool isUpdate = false;
                                    if (result.Name != value.Name)
                                    {
                                        isUpdate = true;
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.Name, PreValue = result?.Name, ColumnName = "Name" });
                                    }
                                    if (result.Abbreviation != value.Abbreviation)
                                    {
                                        isUpdate = true;
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.Abbreviation, PreValue = result?.Abbreviation, ColumnName = "Abbreviation" });
                                    }
                                    if (result.DeparmentId != value.DeparmentId)
                                    {
                                        isUpdate = true;
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.DeparmentId?.ToString(), PreValue = result?.DeparmentId?.ToString(), ColumnName = "DeparmentId" });
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.DepartmentName, PreValue = result?.DepartmentName, ColumnName = "DepartmentName" });

                                    }
                                    if (result.ProfileTypeId != value.ProfileTypeId)
                                    {
                                        isUpdate = true;
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.ProfileTypeId?.ToString(), PreValue = result?.ProfileTypeId?.ToString(), ColumnName = "ProfileTypeId" });
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.ProfileType, PreValue = result?.ProfileType, ColumnName = "ProfileType" });

                                    }
                                    if (result?.GroupId != value?.GroupId)
                                    {
                                        isUpdate = true;
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.GroupId?.ToString(), PreValue = result?.GroupId?.ToString(), ColumnName = "GroupId" });
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.GroupName?.ToString(), PreValue = result?.GroupName?.ToString(), ColumnName = "GroupName" });

                                    }
                                    if (result.GroupAbbreviation != value.GroupAbbreviation)
                                    {
                                        isUpdate = true;
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.GroupAbbreviation, PreValue = result?.GroupAbbreviation, ColumnName = "GroupAbbreviation" });

                                    }
                                    if (result.StatusCodeID != value.StatusCodeID)
                                    {
                                        isUpdate = true;
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.StatusCodeID?.ToString(), PreValue = result?.StatusCodeID?.ToString(), ColumnName = "StatusCodeID" });
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.StatusCode, PreValue = result?.StatusCode, ColumnName = "StatusCode" });
                                    }
                                    if (result.Description != value.Description)
                                    {
                                        isUpdate = true;
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.Description, PreValue = result?.Description, ColumnName = "Description" });

                                    }
                                    if (result.IsEnableTask != value.IsEnableTask)
                                    {
                                        isUpdate = true;
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.IsEnableTask.ToString(), PreValue = value?.IsEnableTask.ToString(), ColumnName = "IsEnableTask" });
                                    }
                                    if (result.SpecialWording != value.SpecialWording)
                                    {
                                        isUpdate = true;
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.SpecialWording, PreValue = result?.SpecialWording, ColumnName = "SpecialWording" });

                                    }
                                    if (result.SeperatorToUse != value.SeperatorToUse)
                                    {
                                        isUpdate = true;
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.SeperatorToUse?.ToString(), PreValue = result?.SeperatorToUse?.ToString(), ColumnName = "SeperatorToUse" });

                                    }
                                    if (result.NoOfDigit != value.NoOfDigit)
                                    {
                                        isUpdate = true;
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.NoOfDigit?.ToString(), PreValue = result?.NoOfDigit?.ToString(), ColumnName = "NoOfDigit" });

                                    }
                                    if (result.IncrementalNo != value.IncrementalNo)
                                    {
                                        isUpdate = true;
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.IncrementalNo?.ToString(), PreValue = result?.IncrementalNo?.ToString(), ColumnName = "IncrementalNo" });

                                    }
                                    if (result.StartingNo != value.StartingNo)
                                    {
                                        isUpdate = true;
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.StartingNo?.ToString(), PreValue = result?.StartingNo?.ToString(), ColumnName = "StartingNo" });

                                    }
                                    if (result.TranslationRequired != value.TranslationRequired)
                                    {
                                        isUpdate = true;
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.TranslationRequired?.ToString(), PreValue = result?.TranslationRequired?.ToString(), ColumnName = "TranslationRequired" });

                                    }
                                    if (result.Note != value.Note)
                                    {
                                        isUpdate = true;
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.Note?.ToString(), PreValue = result?.Note?.ToString(), ColumnName = "Note" });

                                    }
                                    if (result.StartWithYear != value.StartWithYear)
                                    {
                                        isUpdate = true;
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.StartWithYear?.ToString(), PreValue = result?.StartWithYear?.ToString(), ColumnName = "StartWithYear" });

                                    }
                                    if (result.AbbreviationRequired != value.AbbreviationRequired)
                                    {
                                        isUpdate = true;
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.AbbreviationRequired?.ToString(), PreValue = result?.AbbreviationRequired?.ToString(), ColumnName = "AbbreviationRequired" });

                                    }
                                    if (result.IsGroupAbbreviation != value.IsGroupAbbreviation)
                                    {
                                        isUpdate = true;
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.IsGroupAbbreviation?.ToString(), PreValue = result?.IsGroupAbbreviation?.ToString(), ColumnName = "IsGroupAbbreviation" });
                                    }
                                    if (result.IsCategoryAbbreviation != value.IsCategoryAbbreviation)
                                    {
                                        isUpdate = true;
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.IsCategoryAbbreviation?.ToString(), PreValue = result?.IsCategoryAbbreviation?.ToString(), ColumnName = "IsCategoryAbbreviation" });
                                    }
                                    if (result.Abbreviation1 != value.Abbreviation1)
                                    {
                                        string a1 = string.Empty; string a2 = string.Empty;
                                        if (!string.IsNullOrEmpty(result.Abbreviation1))
                                        {
                                            dynamic? abbreviation = JsonConvert.DeserializeObject(result.Abbreviation1);
                                            if (abbreviation != null)
                                            {
                                                foreach (var item in abbreviation)
                                                {
                                                    a1 += item.Name + ",";
                                                }
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(value.Abbreviation1))
                                        {
                                            dynamic? abbreviation = JsonConvert.DeserializeObject(value.Abbreviation1);
                                            if (abbreviation != null)
                                            {
                                                foreach (var item in abbreviation)
                                                {
                                                    a2 += item.Name + ",";
                                                }
                                            }
                                        }
                                        isUpdate = true;
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = a2.TrimEnd(','), PreValue = a1.TrimEnd(','), ColumnName = "Abbreviation1" });

                                    }
                                    if (isUpdate)
                                    {
                                        auditList.Add(new HRMasterAuditTrail { PreValue = value?.Name, CurrentValue = value?.Name, ColumnName = "DisplayName" });
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.ModifiedByUserID?.ToString(), PreValue = result?.ModifiedByUserID?.ToString(), ColumnName = "ModifiedByUserID" });
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.ModifiedDate != null ? value.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, PreValue = result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "ModifiedDate" });
                                        auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.ModifiedByUser?.ToString(), PreValue = result?.ModifiedByUser?.ToString(), ColumnName = "ModifiedBy" });
                                    }
                                    if (auditList.Count() > 0)
                                    {
                                        HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                                        {
                                            HRMasterAuditTrailItems = auditList,
                                            Type = "DocumentProfileNoSeries",
                                            FormType = "Update",
                                            HRMasterSetId = value?.ProfileID,
                                            AuditUserId = value?.ModifiedByUserID,
                                        };
                                        await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
                                    }
                                }
                            }
                            else
                            {
                                value.ProfileID = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);

                                List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
                                auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.Name, ColumnName = "Name" });
                                if (!string.IsNullOrEmpty(value?.Abbreviation))
                                {
                                    auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.Abbreviation, ColumnName = "Abbreviation" });
                                }
                                if (value?.DeparmentId > 0)
                                {
                                    auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.DeparmentId?.ToString(), ColumnName = "DeparmentId" });
                                    auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.DepartmentName?.ToString(), ColumnName = "DepartmentName" });
                                }
                                if (value?.ProfileTypeId > 0)
                                {
                                    auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.ProfileTypeId?.ToString(), ColumnName = "ProfileTypeId" });
                                    auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.ProfileType?.ToString(), ColumnName = "ProfileType" });
                                }

                                if (value?.GroupId > 0)
                                {
                                    auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.GroupId?.ToString(), ColumnName = "GroupId" });
                                    auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.GroupName?.ToString(), ColumnName = "GroupName" });
                                }

                                if (!string.IsNullOrEmpty(value?.GroupAbbreviation))
                                {
                                    auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.GroupAbbreviation, ColumnName = "GroupAbbreviation" });
                                }
                                if (value?.StatusCodeID > 0)
                                {
                                    auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.StatusCodeID?.ToString(), ColumnName = "StatusCodeID" });
                                    auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.StatusCode?.ToString(), ColumnName = "StatusCode" });
                                }
                                if (!string.IsNullOrEmpty(value?.Description))
                                {
                                    auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.Description, ColumnName = "Description" });
                                }
                                if (value?.IsEnableTask == true)
                                {
                                    auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.IsEnableTask.ToString(), ColumnName = "IsEnableTask" });
                                }
                                auditList.Add(new HRMasterAuditTrail { PreValue = value?.Name, CurrentValue = value?.Name, ColumnName = "DisplayName" });
                                auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.AddedByUserID?.ToString(), ColumnName = "AddedByUserID" });
                                auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.AddedDate != null ? value.AddedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "AddedDate" });
                                auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.AddedByUser?.ToString(), ColumnName = "AddedBy" });
                                if (auditList.Count() > 0)
                                {
                                    HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                                    {
                                        HRMasterAuditTrailItems = auditList,
                                        Type = "DocumentProfileNoSeries",
                                        FormType = "Add",
                                        HRMasterSetId = value?.ProfileID,
                                        AuditUserId = value?.AddedByUserID,
                                    };
                                    await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
                                }
                            }
                        }

                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }

                }
                if (value.IsSampleDocNoEnabled == true)
                {
                    // if (string.IsNullOrEmpty(value.SampleDocumentNo))
                    // {
                    var counts = await GetDocumentNoSeriesCount(value.ProfileID);
                    if (counts > 0)
                    {

                    }
                    else
                    {
                        value.SampleDocumentNo = await GenerateSampleDocumentNo(value);
                    }
                    // }
                }
                return value;
            }
            catch (Exception exp)
            {
                throw new NotImplementedException();
            }
        }

    }
}
