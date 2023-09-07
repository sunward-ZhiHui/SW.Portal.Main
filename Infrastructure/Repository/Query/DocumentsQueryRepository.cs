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
        public async Task<long> Delete(long? DocumentId)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {

                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("DocumentID", DocumentId);

                            var query = "DELETE  FROM Documents WHERE DocumentID = @DocumentId";

                            var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception exp)
                        {
                            transaction.Rollback();
                            throw new Exception(exp.Message, exp);
                        }
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
            return await _generateDocumentNoSeriesSeviceQueryRepository.GenerateDocumentProfileAutoNumber(documentNoSeriesModel);
        }
        public async Task<Documents> UpdateDocumentAfterUpload(Documents value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("DocumentId", value.DocumentId);
                            parameters.Add("FileName", value.FileName, DbType.String);
                            parameters.Add("ContentType", value.ContentType, DbType.String);
                            parameters.Add("FileSize", value.FileSize);
                            parameters.Add("FilePath", value.FilePath, DbType.String);
                            var query = "Update Documents SET FileName=@FileName,ContentType=@ContentType,FilePath=@FilePath,FileSize=@FileSize WHERE " +
                                "DocumentId= @DocumentId";
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);
                            transaction.Commit();
                            return value;
                        }
                        catch (Exception exp)
                        {
                            transaction.Rollback();
                            throw new Exception(exp.Message, exp);
                        }
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
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("DocumentId", DocumentId);
                            parameters.Add("IsLatest", 0);
                            var query = "Update Documents SET IsLatest=@IsLatest WHERE DocumentId= @DocumentId";
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);
                            transaction.Commit();
                            return DocumentId;
                        }
                        catch (Exception exp)
                        {
                            transaction.Rollback();
                            throw new Exception(exp.Message, exp);
                        }
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
        public async Task<DocumentsUploadModel> InsertCreateDocument(DocumentsUploadModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
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
                                    value.DocumentMainParentId=value.DocumentParentId;
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
                            var query = "INSERT INTO [Documents](FilterProfileTypeID,Description,ExpiryDate,StatusCodeID,IsTemp,SessionId,ProfileNo,AddedByUserId,IsLatest,AddedDate,UploadDate,IsNewPath,TableName,DocumentParentId,FileIndex) " +
                                "OUTPUT INSERTED.DocumentId VALUES " +
                               "(@FileProfileTypeId,@Description,@ExpiryDate,@StatusCodeID,@IsTemp,@SessionId,@ProfileNo,@AddedByUserId,@IsLatest,@AddedDate,@UploadDate,@IsNewPath,@TableName,@DocumentParentId,@FileIndex)";
                            value.DocumentId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);
                            transaction.Commit();
                            return value;
                        }
                        catch (Exception exp)
                        {
                            transaction.Rollback();
                            throw new Exception(exp.Message, exp);
                        }
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
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("FileName", value.FileName, DbType.String);
                            parameters.Add("ContentType", value.ContentType, DbType.String);
                            parameters.Add("FileSize", value.FileSize);
                            parameters.Add("UploadDate", value.UploadDate, DbType.DateTime);
                            parameters.Add("AddedByUserId", value.AddedByUserId);
                            parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                            parameters.Add("SessionId", value.SessionId, DbType.Guid);
                            parameters.Add("IsLatest", value.IsLatest == true ? 1 : 0, DbType.Boolean);
                            parameters.Add("FilePath", value.FilePath, DbType.String);
                            parameters.Add("IsNewPath", 1);
                            var query = "INSERT INTO [Documents](FileName,ContentType,FileSize,UploadDate,AddedByUserId,AddedDate,SessionId,IsLatest,FilePath,IsNewPath) " +
                                "OUTPUT INSERTED.DocumentId VALUES " +
                               "(@FileName,@ContentType,@FileSize,@UploadDate,@AddedByUserId,@AddedDate,@SessionId,@IsLatest,@FilePath,@IsNewPath)";
                            value.DocumentId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);
                            transaction.Commit();
                            return value;
                        }
                        catch (Exception exp)
                        {
                            transaction.Rollback();
                            throw new Exception(exp.Message, exp);
                        }
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
