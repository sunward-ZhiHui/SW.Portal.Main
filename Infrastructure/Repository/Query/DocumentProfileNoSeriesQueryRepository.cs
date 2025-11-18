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



                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", null, result?.Name, result.ProfileID, guid, UserId, DateTime.Now, true, "Name", uid);
                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result?.Name, result?.Name, result.ProfileID, guid, UserId, DateTime.Now, true, "DisplayName", uid);



                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result?.Abbreviation, null, result.ProfileID, guid, UserId, DateTime.Now, true, "Abbreviation", uid);


                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result?.DeparmentId?.ToString(), null, result.ProfileID, guid, UserId, DateTime.Now, true, "DepartmentId", uid);
                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result.DepartmentName, null, result.ProfileID, guid, UserId, DateTime.Now, true, "DepartmentName", uid);



                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result?.ProfileTypeId?.ToString(), null, result.ProfileID, guid, UserId, DateTime.Now, true, "ProfileTypeId", uid);
                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result?.ProfileType, null, result.ProfileID, guid, UserId, DateTime.Now, true, "ProfileType", uid);



                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result?.GroupId?.ToString(), null, result.ProfileID, guid, UserId, DateTime.Now, true, "GroupId", uid);
                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result.GroupName, null, result.ProfileID, guid, UserId, DateTime.Now, true, "GroupName", uid);



                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result?.GroupAbbreviation, null, result.ProfileID, guid, UserId, DateTime.Now, true, "GroupAbbreviation", uid);



                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result?.StatusCodeID?.ToString(), null, result.ProfileID, guid, UserId, DateTime.Now, true, "StatusCodeID", uid);
                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result?.StatusCode, null, result.ProfileID, guid, UserId, DateTime.Now, true, "StatusCode", uid);
                            uid = Guid.NewGuid();

                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result?.Description, null, result.ProfileID, guid, UserId, DateTime.Now, true, "Description", uid);



                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result?.IsEnableTask?.ToString(), null, result.ProfileID, guid, UserId, DateTime.Now, true, "IsEnableTask", uid);

                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result?.SpecialWording, null, result.ProfileID, guid, UserId, DateTime.Now, true, "SpecialWording", uid);


                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result?.SeperatorToUse?.ToString(), null, result.ProfileID, guid, UserId, DateTime.Now, true, "SeperatorToUse", uid);

                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result?.NoOfDigit?.ToString(), null, result.ProfileID, guid, UserId, DateTime.Now, true, "NoOfDigit", uid);


                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result?.IncrementalNo?.ToString(), null, result.ProfileID, guid, UserId, DateTime.Now, true, "IncrementalNo", uid);


                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result?.StartingNo?.ToString(), null, result.ProfileID, guid, UserId, DateTime.Now, true, "StartingNo", uid);

                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result?.TranslationRequired?.ToString(), null, result.ProfileID, guid, UserId, DateTime.Now, true, "TranslationRequired", uid);


                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result?.Note?.ToString(), null, result.ProfileID, guid, UserId, DateTime.Now, true, "Note", uid);
                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result?.StartWithYear?.ToString(), null, result.ProfileID, guid, UserId, DateTime.Now, true, "StartWithYear", uid);

                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result?.AbbreviationRequired?.ToString(), null, result.ProfileID, guid, UserId, DateTime.Now, true, "AbbreviationRequired", uid);


                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result?.IsGroupAbbreviation?.ToString(), null, result.ProfileID, guid, UserId, DateTime.Now, true, "IsGroupAbbreviation", uid);

                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result?.IsCategoryAbbreviation?.ToString(), null, result.ProfileID, guid, UserId, DateTime.Now, true, "IsCategoryAbbreviation", uid);

                            if (result.Abbreviation1 != null)
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

                                uid = Guid.NewGuid();
                                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", a1.TrimEnd(','), null, result.ProfileID, guid, UserId, DateTime.Now, true, "Abbreviation1", uid);

                            }

                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result?.ModifiedByUserID?.ToString(), null, result.ProfileID, guid, UserId, DateTime.Now, true, "ModifiedByUserID", uid);
                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, null, result.ProfileID, guid, UserId, DateTime.Now, true, "ModifiedDate", uid);
                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Delete", result?.ModifiedByUser, null, result.ProfileID, guid, UserId, DateTime.Now, true, "ModifiedBy", uid);

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
                                var guid = Guid.NewGuid();
                                var uid = Guid.NewGuid();
                                if (result != null)
                                {
                                    bool isUpdate = false;
                                    if (result.Name != value.Name)
                                    {
                                        isUpdate = true;
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result.Name, value?.Name, value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "Name", uid);

                                    }
                                    if (result.Abbreviation != value.Abbreviation)
                                    {
                                        isUpdate = true;
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result?.Abbreviation, value?.Abbreviation, value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "Abbreviation", uid);

                                    }
                                    if (result.DeparmentId != value.DeparmentId)
                                    {
                                        isUpdate = true;
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result?.DeparmentId?.ToString(), value?.DeparmentId?.ToString(), value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "DepartmentId", uid);
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result.DepartmentName, value?.DepartmentName, value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "DepartmentName", uid);

                                    }
                                    if (result.ProfileTypeId != value.ProfileTypeId)
                                    {
                                        isUpdate = true;
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result?.ProfileTypeId?.ToString(), value?.ProfileTypeId?.ToString(), value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "ProfileTypeId", uid);
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result?.ProfileType, value?.ProfileType, value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "ProfileType", uid);

                                    }
                                    if (result.GroupId != value.GroupId)
                                    {
                                        isUpdate = true;
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result?.GroupId?.ToString(), value?.GroupId?.ToString(), value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "GroupId", uid);
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result.GroupName, value?.GroupName, value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "GroupName", uid);

                                    }
                                    if (result.GroupAbbreviation != value.GroupAbbreviation)
                                    {
                                        isUpdate = true;
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result?.GroupAbbreviation, value?.GroupAbbreviation, value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "GroupAbbreviation", uid);

                                    }
                                    if (result.StatusCodeID != value.StatusCodeID)
                                    {
                                        isUpdate = true;
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result?.StatusCodeID?.ToString(), value?.StatusCodeID?.ToString(), value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "StatusCodeID", uid);
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result?.StatusCode, value?.StatusCode, value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "StatusCode", uid);
                                        uid = Guid.NewGuid();
                                    }
                                    if (result.Description != value.Description)
                                    {
                                        isUpdate = true;
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result?.Description, value?.Description, value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "Description", uid);

                                    }
                                    if (result.IsEnableTask != value.IsEnableTask)
                                    {
                                        isUpdate = true;
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result?.IsEnableTask?.ToString(), value?.IsEnableTask.ToString(), value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "IsEnableTask", uid);
                                    }
                                    if (result.SpecialWording != value.SpecialWording)
                                    {
                                        isUpdate = true;
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result?.SpecialWording, value?.SpecialWording, value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "SpecialWording", uid);

                                    }
                                    if (result.SeperatorToUse != value.SeperatorToUse)
                                    {
                                        isUpdate = true;
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result?.SeperatorToUse?.ToString(), value?.SeperatorToUse?.ToString(), value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "SeperatorToUse", uid);

                                    }
                                    if (result.NoOfDigit != value.NoOfDigit)
                                    {
                                        isUpdate = true;
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result?.NoOfDigit?.ToString(), value?.NoOfDigit?.ToString(), value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "NoOfDigit", uid);

                                    }
                                    if (result.IncrementalNo != value.IncrementalNo)
                                    {
                                        isUpdate = true;
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result?.IncrementalNo?.ToString(), value?.IncrementalNo?.ToString(), value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "IncrementalNo", uid);

                                    }
                                    if (result.StartingNo != value.StartingNo)
                                    {
                                        isUpdate = true;
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result?.StartingNo?.ToString(), value?.StartingNo?.ToString(), value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "StartingNo", uid);

                                    }
                                    if (result.TranslationRequired != value.TranslationRequired)
                                    {
                                        isUpdate = true;
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result?.TranslationRequired?.ToString(), value?.TranslationRequired?.ToString(), value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "TranslationRequired", uid);

                                    }
                                    if (result.Note != value.Note)
                                    {
                                        isUpdate = true;
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result?.Note?.ToString(), value?.Note?.ToString(), value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "Note", uid);

                                    }
                                    if (result.StartWithYear != value.StartWithYear)
                                    {
                                        isUpdate = true;
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result?.StartWithYear?.ToString(), value?.StartWithYear?.ToString(), value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "StartWithYear", uid);

                                    }
                                    if (result.AbbreviationRequired != value.AbbreviationRequired)
                                    {
                                        isUpdate = true;
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result?.AbbreviationRequired?.ToString(), value?.AbbreviationRequired?.ToString(), value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "AbbreviationRequired", uid);

                                    }
                                    if (result.IsGroupAbbreviation != value.IsGroupAbbreviation)
                                    {
                                        isUpdate = true;
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result?.IsGroupAbbreviation?.ToString(), value?.IsGroupAbbreviation?.ToString(), value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "IsGroupAbbreviation", uid);
                                    }
                                    if (result.IsCategoryAbbreviation != value.IsCategoryAbbreviation)
                                    {
                                        isUpdate = true;
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result?.IsCategoryAbbreviation?.ToString(), value?.IsCategoryAbbreviation?.ToString(), value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "IsCategoryAbbreviation", uid);
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
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", a1.TrimEnd(','), a2.TrimEnd(','), value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "Abbreviation1", uid);

                                    }
                                    if (isUpdate)
                                    {
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Udate", value?.Name, value?.Name, value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "DisplayName", uid);
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result?.ModifiedByUserID?.ToString(), value?.ModifiedByUserID?.ToString(), value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "ModifiedByUserID", uid);
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, value?.AddedDate != null ? value.AddedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "ModifiedDate", uid);
                                        uid = Guid.NewGuid();
                                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Update", result?.ModifiedByUser, value?.ModifiedByUser?.ToString(), value.ProfileID, guid, value?.ModifiedByUserID, DateTime.Now, false, "ModifiedBy", uid);
                                    }
                                }
                            }
                            else
                            {
                                value.ProfileID = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                                var guid = Guid.NewGuid();
                                var uid = Guid.NewGuid();
                                if (!string.IsNullOrEmpty(value.Name))
                                {
                                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Add", null, value?.Name, value.ProfileID, guid, value?.AddedByUserID, DateTime.Now, false, "Name", uid);

                                }
                                if (!string.IsNullOrEmpty(value.Abbreviation))
                                {
                                    uid = Guid.NewGuid();
                                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Add", null, value?.Abbreviation, value.ProfileID, guid, value?.AddedByUserID, DateTime.Now, false, "Abbreviation", uid);

                                }
                                if (value.DeparmentId > 0)
                                {
                                    uid = Guid.NewGuid();
                                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Add", null, value?.DeparmentId?.ToString(), value.ProfileID, guid, value?.AddedByUserID, DateTime.Now, false, "DepartmentId", uid);
                                    uid = Guid.NewGuid();
                                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Add", null, value?.DepartmentName, value.ProfileID, guid, value?.AddedByUserID, DateTime.Now, false, "DepartmentName", uid);

                                }
                                if (value.ProfileTypeId > 0)
                                {
                                    uid = Guid.NewGuid();
                                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Add", null, value?.ProfileTypeId?.ToString(), value.ProfileID, guid, value?.AddedByUserID, DateTime.Now, false, "ProfileTypeId", uid);
                                    uid = Guid.NewGuid();
                                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Add", null, value?.ProfileType, value.ProfileID, guid, value?.AddedByUserID, DateTime.Now, false, "ProfileType", uid);

                                }
                                if (value.GroupId > 0)
                                {
                                    uid = Guid.NewGuid();
                                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Add", null, value?.GroupId?.ToString(), value.ProfileID, guid, value?.AddedByUserID, DateTime.Now, false, "GroupId", uid);
                                    uid = Guid.NewGuid();
                                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Add", null, value?.GroupName, value.ProfileID, guid, value?.AddedByUserID, DateTime.Now, false, "GroupName", uid);

                                }
                                if (!string.IsNullOrEmpty(value.GroupAbbreviation))
                                {
                                    uid = Guid.NewGuid();
                                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Add", null, value?.GroupAbbreviation, value.ProfileID, guid, value?.AddedByUserID, DateTime.Now, false, "GroupAbbreviation", uid);

                                }
                                if (value.StatusCodeID > 0)
                                {
                                    uid = Guid.NewGuid();
                                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Add", null, value?.StatusCodeID?.ToString(), value.ProfileID, guid, value?.AddedByUserID, DateTime.Now, false, "StatusCodeID", uid);
                                    uid = Guid.NewGuid();
                                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Add", null, value?.StatusCode, value.ProfileID, guid, value?.AddedByUserID, DateTime.Now, false, "StatusCode", uid);
                                    uid = Guid.NewGuid();
                                }
                                if (!string.IsNullOrEmpty(value.Description))
                                {
                                    uid = Guid.NewGuid();
                                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Add", null, value?.Description, value.ProfileID, guid, value?.AddedByUserID, DateTime.Now, false, "Description", uid);

                                }
                                uid = Guid.NewGuid();
                                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Add", value?.Name, value?.Name, value.ProfileID, guid, value?.AddedByUserID, DateTime.Now, false, "DisplayName", uid);

                                uid = Guid.NewGuid();
                                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Add", null, value?.IsEnableTask.ToString(), value.ProfileID, guid, value?.AddedByUserID, DateTime.Now, false, "IsEnableTask", uid);
                                uid = Guid.NewGuid();
                                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Add", null, value?.AddedByUserID?.ToString(), value.ProfileID, guid, value?.AddedByUserID, DateTime.Now, false, "AddedByUserID", uid);
                                uid = Guid.NewGuid();
                                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Add", null, value?.AddedDate != null ? value.AddedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, value.ProfileID, guid, value?.AddedByUserID, DateTime.Now, false, "AddedDate", uid);
                                uid = Guid.NewGuid();
                                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("DocumentProfileNoSeries", "Add", null, value?.AddedByUser?.ToString(), value.ProfileID, guid, value?.AddedByUserID, DateTime.Now, false, "AddedBy", uid);

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
