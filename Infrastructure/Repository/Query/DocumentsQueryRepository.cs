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
using IdentityModel.Client;
using Application.Common.Helper;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using Core.EntityModels;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Repository.Query
{
    public class DocumentsQueryRepository : QueryRepository<Documents>, IDocumentsQueryRepository
    {
        private readonly IGenerateDocumentNoSeriesSeviceQueryRepository _generateDocumentNoSeriesSeviceQueryRepository;
        public DocumentsQueryRepository(IConfiguration configuration, IGenerateDocumentNoSeriesSeviceQueryRepository generateDocumentNoSeriesSeviceQueryRepository)
            : base(configuration)
        {
            _generateDocumentNoSeriesSeviceQueryRepository = generateDocumentNoSeriesSeviceQueryRepository;
        }
        public async Task<byte[]> GetByteFromUrl(string url)
        {
            var webClient = new WebClient();
            {
                byte[] bytesData = await webClient.DownloadDataTaskAsync(new Uri(url));
                return bytesData;
            }
        }
        public async Task<Documents> GetBySessionIdAsync(Guid? SessionId)
        {
            try
            {
                var query = "SELECT DocumentId,IsNewPath,FilePath FROM Documents WHERE IsLatest= 1 and  SessionId = @SessionId";
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<Documents>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public Documents GetByOneAsync(long? DocumentId)
        {
            try
            {
                var query = "SELECT  DocumentId,IsNewPath,FilePath FROM Documents WHERE DocumentId = @DocumentId";
                var parameters = new DynamicParameters();
                parameters.Add("DocumentId", DocumentId);
                using (var connection = CreateConnection())
                {
                    return connection.QueryFirstOrDefault<Documents>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<Documents> GetByIdAsync(long? DocumentId)
        {
            try
            {
                var query = "SELECT * FROM Documents WHERE IsLatest= 1 and DocumentId = @DocumentId";
                var parameters = new DynamicParameters();
                parameters.Add("DocumentId", DocumentId);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<Documents>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<List<Documents>> GetByUniqueDocAsync(string Ids)
        {
            try
            {
                //var query = "select EC.Name AS SubjectName,\r\n\tD.DocumentID as DocumentId,\r\n\tD.DocumentID as ReplaceDocumentId,\r\n\tD.FileName,\r\n\tD.ContentType,\r\n\tD.FileSize,\r\n\tD.UploadDate,\r\n\tD.SessionID,\r\n\tD.AddedDate,\r\n\tD.FilePath,\t\r\n\tD.AddedDate,\t\r\n\tD.ModifiedDate,\r\n\tD.UniqueSessionId,\r\n\tD.EmailToDMS from Documents D LEFT JOIN EmailConversations EC ON EC.SessionId = D.SessionID where D.SessionID in\r\n(select DISTINCT SessionId from ActivityEmailTopics where ActivityEmailTopicID in (select EmailToDMS  from Documents where DocumentID in(" + Ids + ")))\r\nAND D.IsLatest = 1";
                //var idList = Ids.Split(',').Select(id => Convert.ToInt64(id.Trim()));

                var idList = Ids.Split(',').Select(id => Convert.ToInt64(id.Trim()));

                var dataTable = new DataTable();
                dataTable.Columns.Add("Id", typeof(long));

                foreach (var id in idList)
                {
                    dataTable.Rows.Add(id);
                }

                var query = @" select 
                                DISTINCT		                       
		                        DD.DocumentID as DocumentId,
		                        DD.DocumentID as ReplaceDocumentId,
                                EC.Name AS SubjectName,
		                        DD.FileName,
		                        DD.ContentType,
		                        DD.FileSize,
		                        DD.UploadDate,
		                        DD.SessionID,
		                        DD.AddedDate,
		                        DD.FilePath,	
		                        DD.AddedDate,	
		                        DD.ModifiedDate,
		                        DD.UniqueSessionId,
		                        DD.EmailToDMS 
	                        From Documents D 
	                        Inner Join EmailConversations EC On EC.SessionId=D.SessionID
	                        Inner Join ActivityEmailTopics AE On D.EmailToDMS=AE.ActivityEmailTopicID 
	                        Inner Join Documents DD On DD.SessionID=AE.SessionId 
	                        Where DD.IsLatest = 1 AND D.DocumentID IN (SELECT Id FROM @IdList)";
                // var parameters = new DynamicParameters();
                // parameters.Add("Ids", Ids);
                //var parameters = new { Ids = idList.ToArray() };
                //var parameters = new { IdList = idList.Select(id => new { Id = id }).ToList() };

                var parameters = new
                {
                    IdList = dataTable.AsTableValuedParameter("dbo.IdListType")
                };

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<Documents>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<Documents> GetByDocIdAsync(long? DocumentId)
        {
            try
            {
                var query = "SELECT * FROM Documents WHERE DocumentId = @DocumentId";
                var parameters = new DynamicParameters();
                parameters.Add("DocumentId", DocumentId);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<Documents>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> Delete(long? DocumentId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DocumentID", DocumentId);
                        var query = "DELETE  FROM Documents WHERE DocumentID = @DocumentId";

                        var rowsAffected = await connection.ExecuteAsync(query, parameters);

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
        private async Task<string> GenerateDocumentProfileAutoNumber(DocumentsUploadModel value)
        {
            DocumentNoSeriesModel documentNoSeriesModel = new DocumentNoSeriesModel();
            documentNoSeriesModel.AddedByUserID = value.UserId;
            documentNoSeriesModel.StatusCodeID = 710;
            documentNoSeriesModel.ProfileID = value.ProfileId;
            documentNoSeriesModel.PlantID = value.PlantId;
            documentNoSeriesModel.DepartmentId = value.DepartmentId;
            documentNoSeriesModel.SectionId = value.SectionId;
            documentNoSeriesModel.SubSectionId = value.SubSectionId;
            documentNoSeriesModel.DivisionId = value.DivisionId;
            documentNoSeriesModel.CompanyCode = value.CompanyCode;
            documentNoSeriesModel.SectionName = value.SectionName;
            documentNoSeriesModel.SubSectionName = value.SubSectionName;
            documentNoSeriesModel.DepartmentName = value.DepartmentName;
            documentNoSeriesModel.FileProfileTypeId = value.FileProfileTypeId;
            documentNoSeriesModel.NoOfCounts = value.DocumentsUploadModels.Count();
            documentNoSeriesModel.SessionId = value.FileSessionId;
            return await _generateDocumentNoSeriesSeviceQueryRepository.GenerateDocumentProfileAutoNumber(documentNoSeriesModel);
        }

        public async Task<long> UpdateEmailDMS(long DocId, long ActivityId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DocId", DocId);
                        parameters.Add("ActivityId", ActivityId);
                        var query = @"Update Documents SET EmailToDMS=@ActivityId WHERE DocumentId= @DocId";
                        var rowsAffected = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
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
        public async Task<Documents> UpdateDocumentAfterUpload(Documents value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DocumentId", value.DocumentId);
                        parameters.Add("FileName", value.FileName, DbType.String);
                        parameters.Add("ContentType", value.ContentType, DbType.String);
                        parameters.Add("FileSize", value.FileSize);
                        parameters.Add("FilePath", value.FilePath, DbType.String);
                        parameters.Add("SourceFrom", value.SourceFrom, DbType.String);
                        var query = "Update Documents SET FileName=@FileName,ContentType=@ContentType,FilePath=@FilePath,FileSize=@FileSize,SourceFrom=@SourceFrom WHERE " +
                            "DocumentId= @DocumentId";
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        return value;
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
        public async Task<long?> UpdateDocumentIsLastet(long? DocumentId)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DocumentId", DocumentId);
                        parameters.Add("IsLatest", 0);
                        var query = "Update Documents SET IsLatest=@IsLatest WHERE DocumentId= @DocumentId";
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        return DocumentId;
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
        public async Task<long?> GetDocumentParentCount(long? DocumentId)
        {
            try
            {
                var query = "SELECT documentId FROM Documents WHERE DocumentParentId = @DocumentParentId";
                var parameters = new DynamicParameters();
                parameters.Add("DocumentParentId", DocumentId);
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<Documents>(query, parameters)).ToList();
                    return result.Count();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<Documents> GetDocumentIdByPathName(DocumentsUploadModel value)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("AddedByUserId", value.UserId);
                parameters.Add("SessionId", value.FileSessionId, DbType.Guid);
                parameters.Add("FilePath", value.FilePath, DbType.String);
                using (var connection = CreateConnection())
                {
                    var query = "SELECT documentId,sessionId FROM Documents WHERE AddedByUserId = @AddedByUserId AND SessionId=@SessionId AND FilePath=@FilePath";
                    var result = await connection.QueryFirstOrDefaultAsync<Documents>(query, parameters);
                    return result;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DocumentsUploadModel> InsertCreateDocument(DocumentsUploadModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        Nullable<long> fileIndex = null;
                        var parameters = new DynamicParameters();
                        var profileNo = value.ProfileNo;
                        if (value.IsCheckOut == true && value.DocumentParentId > 0)
                        {
                            if (value.DocumentMainParentId > 0)
                            {
                                fileIndex = await GetDocumentParentCount(value.DocumentMainParentId);
                                fileIndex = fileIndex > 0 ? (fileIndex.Value + 1) : 1;
                            }
                            else
                            {
                                value.DocumentMainParentId = value.DocumentParentId;
                                fileIndex = 1;
                            }
                            await UpdateDocumentIsLastet(value.DocumentParentId);
                        }
                        else
                        {
                            profileNo = await GenerateDocumentProfileAutoNumber(value);
                        }
                        parameters.Add("FileProfileTypeId", value.FileProfileTypeId);
                        parameters.Add("Description", value.Description);
                        parameters.Add("ExpiryDate", value.ExpiryDate);
                        parameters.Add("IsTemp", 1);
                        parameters.Add("SessionId", value.SessionId, DbType.Guid);
                        parameters.Add("StatusCodeID", 1);
                        parameters.Add("ProfileNo", profileNo);
                        parameters.Add("AddedByUserId", value.UserId);
                        parameters.Add("IsLatest", 1);
                        parameters.Add("IsNewPath", 1);
                        parameters.Add("DocumentParentId", value.DocumentMainParentId);
                        parameters.Add("TableName", "Document");
                        parameters.Add("AddedDate", DateTime.Now);
                        parameters.Add("UploadDate", DateTime.Now);
                        parameters.Add("FileIndex", fileIndex);
                        parameters.Add("SourceFrom", value.SourceFrom, DbType.String);
                        var query = "INSERT INTO [Documents](FilterProfileTypeID,Description,ExpiryDate,StatusCodeID,IsTemp,SessionId,ProfileNo,AddedByUserId,IsLatest,AddedDate,UploadDate,IsNewPath,TableName,DocumentParentId,FileIndex,SourceFrom) " +
                            "OUTPUT INSERTED.DocumentId VALUES " +
                           "(@FileProfileTypeId,@Description,@ExpiryDate,@StatusCodeID,@IsTemp,@SessionId,@ProfileNo,@AddedByUserId,@IsLatest,@AddedDate,@UploadDate,@IsNewPath,@TableName,@DocumentParentId,@FileIndex,@SourceFrom)";
                        value.DocumentId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        return value;
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
        public async Task<Documents> InsertCreateDocumentBySession(Documents value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("FilterProfileTypeID", value.FilterProfileTypeId);
                        parameters.Add("FileName", value.FileName, DbType.String);
                        parameters.Add("ContentType", value.ContentType, DbType.String);
                        parameters.Add("FileSize", value.FileSize);
                        parameters.Add("UploadDate", value.UploadDate, DbType.DateTime);
                        parameters.Add("AddedByUserId", value.AddedByUserId);
                        parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                        parameters.Add("ModifiedByUserID", value.AddedByUserId);
                        parameters.Add("ModifiedDate", value.AddedDate, DbType.DateTime);
                        parameters.Add("SessionId", value.SessionId, DbType.Guid);
                        parameters.Add("IsLatest", 1);
                        parameters.Add("FilePath", value.FilePath, DbType.String);
                        parameters.Add("IsNewPath", 1);
                        parameters.Add("IsTemp", value.IsTemp);
                        parameters.Add("ProfileNo", value.ProfileNo, DbType.String);
                        parameters.Add("SourceFrom", value.SourceFrom, DbType.String);
                        var query = "INSERT INTO [Documents](FilterProfileTypeID,FileName,ContentType,FileSize,UploadDate,AddedByUserId,AddedDate,ModifiedByUserID,ModifiedDate,SessionId,IsLatest,FilePath,IsNewPath,IsTemp,SourceFrom,ProfileNo) " +
                            "OUTPUT INSERTED.DocumentId VALUES " +
                           "(@FilterProfileTypeID,@FileName,@ContentType,@FileSize,@UploadDate,@AddedByUserId,@AddedDate,@ModifiedByUserID,@ModifiedDate,@SessionId,@IsLatest,@FilePath,@IsNewPath,@IsTemp,@SourceFrom,@ProfileNo)";
                        value.DocumentId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);

                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                    return value;
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DocumentsUploadModel> UpdateCreateDocumentBySession(DocumentsUploadModel value)
        {

            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        Nullable<long> fileIndex = null;
                        var parameters = new DynamicParameters();
                        var profileNo = value.ProfileNo;
                        if (value.IsCheckOut == true && value.DocumentParentId > 0)
                        {
                            if (value.DocumentMainParentId > 0)
                            {
                                fileIndex = await GetDocumentParentCount(value.DocumentMainParentId);
                                fileIndex = fileIndex > 0 ? (fileIndex.Value + 1) : 1;
                            }
                            else
                            {
                                value.DocumentMainParentId = value.DocumentParentId;
                                fileIndex = 1;
                            }
                            await UpdateDocumentIsLastet(value.DocumentParentId);
                        }
                        else
                        {

                            profileNo = await GenerateDocumentProfileAutoNumber(value);

                        }
                        parameters.Add("FileProfileTypeId", value.FileProfileTypeId);
                        parameters.Add("Description", value.Description);
                        parameters.Add("ExpiryDate", value.ExpiryDate);
                        parameters.Add("StatusCodeID", 1);
                        parameters.Add("AddedByUserId", value.UserId);
                        parameters.Add("SessionId", value.SessionId, DbType.Guid);
                        parameters.Add("ProfileNo", profileNo);
                        parameters.Add("DocumentParentId", value.DocumentMainParentId);
                        parameters.Add("TableName", "Document");
                        parameters.Add("FileIndex", fileIndex);
                        parameters.Add("FileSessionId", value.FileSessionId);
                        parameters.Add("IsTemp", 0);
                        parameters.Add("FilePath", value.FilePath, DbType.String);

                        var query = "Update Documents SET " +
                            "FilterProfileTypeId=@FileProfileTypeId, " +
                            "Description=@Description, " +
                            "ExpiryDate=@ExpiryDate, " +
                            "StatusCodeID=@StatusCodeID, ";

                        query += "ProfileNo=@ProfileNo, ";

                        query += "DocumentParentId=@DocumentParentId, " +
                            "TableName=@TableName, " +
                            "FileIndex=@FileIndex, " +
                            "IsTemp=@IsTemp, " +

                            "SessionId=@SessionId " +
                            "WHERE " +
                             "AddedByUserId=@AddedByUserId AND " +
                             "SessionId=@FileSessionId AND " +
                            "FilePath= @FilePath";
                        await connection.ExecuteAsync(query, parameters);
                        connection.Close();
                        if (value.Type == "Document Link")
                        {
                            await InsertDocumentLink(value);
                        }
                        if (value.Type == "Production Activity")
                        {
                            await InsertSupportingDocumentLink(value);
                        }
                        return value;
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
        public async Task<DocumentsUploadModel> UpdateEmailDocumentBySession(DocumentsUploadModel value)
        {

            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        Nullable<long> fileIndex = null;
                        var parameters = new DynamicParameters();
                        var profileNo = value.ProfileNo;
                        if (value.IsCheckOut == true && value.DocumentParentId > 0)
                        {
                            if (value.DocumentMainParentId > 0)
                            {
                                fileIndex = await GetDocumentParentCount(value.DocumentMainParentId);
                                fileIndex = fileIndex > 0 ? (fileIndex.Value + 1) : 1;
                            }
                            else
                            {
                                value.DocumentMainParentId = value.DocumentParentId;
                                fileIndex = 1;
                            }
                            await UpdateDocumentIsLastet(value.DocumentParentId);
                        }
                        else
                        {
                            profileNo = await GenerateDocumentProfileAutoNumber(value);
                        }
                        parameters.Add("DocumentId", value.DocumentId);
                        parameters.Add("FileProfileTypeId", value.FileProfileTypeId);
                        parameters.Add("Description", value.Description);
                        parameters.Add("ExpiryDate", value.ExpiryDate);
                        parameters.Add("StatusCodeID", 1);
                        parameters.Add("AddedByUserId", value.UserId);
                        parameters.Add("SessionId", value.SessionId, DbType.Guid);
                        parameters.Add("ProfileNo", profileNo);
                        parameters.Add("DocumentParentId", value.DocumentMainParentId);
                        parameters.Add("TableName", "Document");
                        parameters.Add("FileIndex", fileIndex);
                        parameters.Add("FileSessionId", value.FileSessionId);
                        parameters.Add("IsTemp", 0);
                        parameters.Add("FilePath", value.FilePath, DbType.String);

                        var query = "Update Documents SET " +
                            "FilterProfileTypeId=@FileProfileTypeId, " +
                            "Description=@Description, " +
                            "ExpiryDate=@ExpiryDate, " +
                            "StatusCodeID=@StatusCodeID, " +
                            "ProfileNo=@ProfileNo, " +
                            "DocumentParentId=@DocumentParentId, " +
                            "TableName=@TableName, " +
                            "FileIndex=@FileIndex, " +
                            "IsTemp=@IsTemp, " +
                            "SessionId=@SessionId " +
                            "WHERE " +
                            "DocumentID= @DocumentId";
                        await connection.ExecuteAsync(query, parameters);
                        connection.Close();
                        return value;
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
        public async Task<DocumentsUploadModel> InsertSupportingDocumentLink(DocumentsUploadModel value)
        {

            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var DocuId = await GetDocumentIdByPathName(value);
                        if (DocuId != null)
                        {
                            var LinkDocparameters = new DynamicParameters();
                            LinkDocparameters.Add("ProductionActivityAppLineId", value.ProductionActivityAppLineId);
                            LinkDocparameters.Add("SessionId", DocuId.SessionId, DbType.Guid);
                            LinkDocparameters.Add("Type", value.Type, DbType.String);
                            LinkDocparameters.Add("DocumentId", DocuId.DocumentId);
                            var linkquery = "INSERT INTO [ProductionActivityAppLineDoc](ProductionActivityAppLineId,Type,DocumentId) " +
                           "OUTPUT INSERTED.ProductionActivityAppLineDocId VALUES " +
                          "(@ProductionActivityAppLineId,@Type,@DocumentId)";
                            await connection.ExecuteAsync(linkquery, LinkDocparameters);
                        }
                        connection.Close();
                        return value;
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
        public async Task<DocumentsUploadModel> InsertDocumentLink(DocumentsUploadModel value)
        {

            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var DocuId = await GetDocumentIdByPathName(value);
                        if (DocuId != null)
                        {
                            var LinkDocparameters = new DynamicParameters();
                            LinkDocparameters.Add("FileProfieTypeId", value.FileProfileTypeId);
                            LinkDocparameters.Add("AddedByUserId", value.UserId);
                            LinkDocparameters.Add("AddedDate", DateTime.Now);
                            LinkDocparameters.Add("DocumentPath", value.Type);
                            LinkDocparameters.Add("LinkDocumentId", DocuId.DocumentId);
                            LinkDocparameters.Add("DocumentId", value.DocumentId);
                            var linkquery = "INSERT INTO [DocumentLink](FileProfieTypeId,AddedByUserId,AddedDate,DocumentPath,LinkDocumentId,DocumentId) " +
                           "OUTPUT INSERTED.DocumentLinkId VALUES " +
                          "(@FileProfieTypeId,@AddedByUserId,@AddedDate,@DocumentPath,@LinkDocumentId,@DocumentId)";
                            await connection.ExecuteAsync(linkquery, LinkDocparameters);
                        }

                        connection.Close();
                        return value;
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
        public async Task<DocumentNoSeriesModel> InsertOrUpdateReserveProfileNumberSeries(DocumentNoSeriesModel documentNoSeries)
        {
            documentNoSeries.DocumentNo = await _generateDocumentNoSeriesSeviceQueryRepository.GenerateDocumentProfileAutoNumber(documentNoSeries);
            return documentNoSeries;
        }
        public async Task<DocumentNoSeriesModel> UpdateCreateDocumentBySessionReserveSeries(DocumentNoSeriesModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        Nullable<long> fileIndex = null;
                        var parameters = new DynamicParameters();
                        parameters.Add("FileProfileTypeId", value.FileProfileTypeId);
                        parameters.Add("Description", value.Description);
                        parameters.Add("StatusCodeID", value.StatusCodeID);
                        parameters.Add("AddedByUserId", value.AddedByUserID);
                        parameters.Add("SessionId", value.SessionID, DbType.Guid);
                        parameters.Add("ProfileNo", value.DocumentNo);
                        parameters.Add("TableName", "Document");
                        parameters.Add("FileIndex", fileIndex);
                        parameters.Add("IsTemp", 0);
                        parameters.Add("FilePath", value.FilePath, DbType.String);
                        var query = "Update Documents SET " +
                            "FilterProfileTypeId=@FileProfileTypeId, " +
                            "Description=@Description, " +
                            "StatusCodeID=@StatusCodeID, " +
                            "ProfileNo=@ProfileNo, " +
                            "TableName=@TableName, " +
                            "FileIndex=@FileIndex, " +
                            "IsTemp=@IsTemp " +
                            "WHERE " +
                             "AddedByUserId=@AddedByUserId AND " +
                             "SessionId=@SessionID AND " +
                             "FilePath= @FilePath;";

                        parameters.Add("ModifiedByUserID", value.AddedByUserID);
                        parameters.Add("ModifiedDate", DateTime.Now);
                        parameters.Add("IsUpload", 1);
                        parameters.Add("NumberSeriesId", value.NumberSeriesId);
                        query += "Update DocumentNoSeries SET FileProfileTypeID=@FileProfileTypeID,ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,IsUpload=@IsUpload WHERE NumberSeriesId= @NumberSeriesId;";
                        await connection.ExecuteAsync(query, parameters);
                        return value;
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
        public async Task<DocumentNoSeriesModel> UpdateReserveNumberDescriptionField(DocumentNoSeriesModel value)
        {

            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {

                        var LinkDocparameters = new DynamicParameters();
                        LinkDocparameters.Add("Description", value.Description);
                        LinkDocparameters.Add("ModifiedByUserID", value.AddedByUserID);
                        LinkDocparameters.Add("ModifiedDate", DateTime.Now);
                        LinkDocparameters.Add("NumberSeriesId", value.NumberSeriesId);
                        var query = "Update DocumentNoSeries SET Description=@Description,ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate WHERE NumberSeriesId= @NumberSeriesId";
                        await connection.ExecuteAsync(query, LinkDocparameters);
                        return value;
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
        private async Task<GenerateDocumentNoSeriesModel> GenerateDocumentProfileAutoNumberOne(DocumentsUploadModel value)
        {
            DocumentNoSeriesModel documentNoSeriesModel = new DocumentNoSeriesModel();
            documentNoSeriesModel.AddedByUserID = value.UserId;
            documentNoSeriesModel.StatusCodeID = 710;
            documentNoSeriesModel.ProfileID = value.ProfileId;
            documentNoSeriesModel.PlantID = value.PlantId;
            documentNoSeriesModel.DepartmentId = value.DepartmentId;
            documentNoSeriesModel.SectionId = value.SectionId;
            documentNoSeriesModel.SubSectionId = value.SubSectionId;
            documentNoSeriesModel.DivisionId = value.DivisionId;
            documentNoSeriesModel.CompanyCode = value.CompanyCode;
            documentNoSeriesModel.SectionName = value.SectionName;
            documentNoSeriesModel.SubSectionName = value.SubSectionName;
            documentNoSeriesModel.DepartmentName = value.DepartmentName;
            documentNoSeriesModel.FileProfileTypeId = value.FileProfileTypeId;
            documentNoSeriesModel.SessionId = value.FileSessionId;
            documentNoSeriesModel.NoOfCounts = value.DocumentsUploadModels.Count();
            return await _generateDocumentNoSeriesSeviceQueryRepository.GenerateDocumentProfileAutoNumberAllAsync(documentNoSeriesModel);
        }
        public async Task<List<Documents>> getDocIds(DocumentsUploadModel value, List<string?> filePaths)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("AddedByUserId", value.UserId);
                using (var connection = CreateConnection())
                {
                    var query = "SELECT documentId FROM Documents WHERE AddedByUserId = @AddedByUserId AND FilePath in(" + string.Join(",", filePaths.Select(x => string.Format("'{0}'", x.ToString().Replace("'", "''")))) + ")";
                    var result = (await connection.QueryAsync<Documents>(query, parameters)).ToList();
                    return result;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DocumentsUploadModel> UpdateDocumentNoDocumentBySession(DocumentsUploadModel values)
        {

            try
            {
                var query = string.Empty;

                if (values.DocumentsUploadModels != null && values.DocumentsUploadModels.Count > 0)
                {
                    //var filePaths = values.DocumentsUploadModels.Select(s => s.FilePath).ToList();
                    // var docIds = await getDocIds(values, filePaths);
                    var profileNos = await GenerateDocumentProfileAutoNumberOne(values);
                    int? lastNoUsed = Convert.ToInt32(profileNos.ProfileAutoNumber.LastNoUsed == null ? null : profileNos.ProfileAutoNumber.LastNoUsed);
                    int? incrementNo = null;
                    if (profileNos.DocumentProfileNoSeries.ProfileId > 0)
                    {
                        var startNo = Convert.ToInt32(!string.IsNullOrEmpty(profileNos.DocumentProfileNoSeries.StartingNo) ? profileNos.DocumentProfileNoSeries.StartingNo : 0);
                        incrementNo = profileNos.DocumentProfileNoSeries.ProfileId > 0 ? ((startNo + profileNos.DocumentProfileNoSeries.IncrementalNo.GetValueOrDefault(0))) : null;
                    }
                    values.DocumentsUploadModels.ForEach(async value =>
                {
                    if (incrementNo != null)
                    {
                        lastNoUsed += incrementNo;
                    }
                    var parameters = new DynamicParameters();
                    parameters.Add("FileProfileTypeId", values.FileProfileTypeId);
                    parameters.Add("Description", values.Description);
                    parameters.Add("ExpiryDate", values.ExpiryDate);
                    parameters.Add("StatusCodeID", 1);
                    parameters.Add("AddedByUserId", values.UserId);
                    parameters.Add("TableName", "Document");
                    parameters.Add("IsTemp", 0);
                    parameters.Add("FilePath", value.FilePath);
                    parameters.Add("SessionId", value.SessionId);
                    parameters.Add("FileSessionId", value.FileSessionId);
                    var profileNo = profileNos.ProfileNo;
                    if (lastNoUsed > 0 && profileNos.DocumentProfileNoSeries.NoOfDigit > 0)
                    {
                        profileNo += Convert.ToInt32(lastNoUsed).ToString("D" + profileNos.DocumentProfileNoSeries.NoOfDigit);
                    }
                    parameters.Add("ProfileNo", profileNo);

                    await UpdateDocumentNoDocumentBySessions(parameters);
                    await InsertDocumentNoSeries(values, profileNo, value.SessionId);
                    if (values.Type == "Document Link")
                    {
                        values.FileSessionId = value.FileSessionId;
                        values.FilePath = value.FilePath;
                        await InsertDocumentLink(values);
                    }
                    if (values.Type == "Production Activity")
                    {
                        values.SessionId = value.FileSessionId;
                        values.FileSessionId = value.FileSessionId;
                        values.FilePath = value.FilePath;
                        await InsertSupportingDocumentLink(values);
                    }
                });
                }

                return values;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        private async Task<DocumentsUploadModel> InsertDocumentNoSeries(DocumentsUploadModel documentNoSeries, string? DocumentNo, Guid? SessionId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ProfileId", documentNoSeries.ProfileId);
                        parameters.Add("DocumentNo", DocumentNo, DbType.String);
                        parameters.Add("AddedDate", DateTime.Now, DbType.DateTime);
                        parameters.Add("AddedByUserID", documentNoSeries.UserId);
                        parameters.Add("StatusCodeId", 710);
                        parameters.Add("SessionId", SessionId, DbType.Guid);
                        parameters.Add("RequestorId", documentNoSeries.UserId);
                        parameters.Add("ModifiedDate", DateTime.Now);
                        parameters.Add("FileProfileTypeId", documentNoSeries.FileProfileTypeId);
                        parameters.Add("ModifiedByUserId", documentNoSeries.UserId);
                        parameters.Add("Description", documentNoSeries.Description, DbType.String);
                        var query = "INSERT INTO [DocumentNoSeries](ProfileId,DocumentNo,AddedDate,AddedByUserID,StatusCodeId," +
                            "SessionId,RequestorId,ModifiedDate,ModifiedByUserId,FileProfileTypeId,Description) " +
                            "OUTPUT INSERTED.NumberSeriesId VALUES " +
                           "(@ProfileId,@DocumentNo,@AddedDate,@AddedByUserID,@StatusCodeId,@SessionId,@RequestorId," +
                           "@ModifiedDate,@ModifiedByUserId,@FileProfileTypeId,@Description)";
                        await connection.QueryFirstOrDefaultAsync<long>(query, parameters);

                        return documentNoSeries;
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
        private async Task<DocumentsUploadModel> UpdateDocumentNoDocumentBySessions(DynamicParameters parameters)
        {
            DocumentsUploadModel documentsUploadModel = new DocumentsUploadModel();
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var query = string.Empty;
                        query += "Update Documents SET " +
                            "FilterProfileTypeId=@FileProfileTypeId, " +
                            "Description=@Description, " +
                            "ExpiryDate=@ExpiryDate, " +
                            "StatusCodeID=@StatusCodeID, " +
                            "ProfileNo=@ProfileNo, " +
                            "TableName=@TableName, " +
                            "IsTemp=@IsTemp, " +
                            "SessionId=@SessionId " +
                            "WHERE " +
                             "AddedByUserId=@AddedByUserId AND " +
                             "SessionId=@FileSessionId  AND " +
                            "FilePath=@FilePath;\n\r";
                        if (!string.IsNullOrEmpty(query))
                        {
                            await connection.ExecuteAsync(query, parameters);
                        }

                        return documentsUploadModel;
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
    }
}
