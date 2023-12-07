using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Query
{
    public class GenerateDocumentNoSeriesQueryRepository : QueryRepository<DocumentNoSeriesModel>, IGenerateDocumentNoSeriesQueryRepository
    {
        public GenerateDocumentNoSeriesQueryRepository(IConfiguration configuration) : base(configuration)
        {

        }
        public async Task<MasterListsModel> GetMasterLists(DocumentNoSeriesModel documentNoSeriesModel)
        {
            try
            {
                MasterListsModel masterLists = new MasterListsModel();
                using (var connection = CreateConnection())
                {
                    var query = string.Empty;

                    query += "SELECT * FROM Plant where PlantID=" + (documentNoSeriesModel.PlantID > 0 ? documentNoSeriesModel.PlantID : 0) + ";";
                    query += "SELECT * FROM Department where DepartmentId=" + (documentNoSeriesModel.DepartmentId > 0 ? documentNoSeriesModel.DepartmentId : 0) + ";";
                    query += "SELECT * FROM Section where SectionId=" + (documentNoSeriesModel.SectionId > 0 ? documentNoSeriesModel.SectionId : 0) + ";";
                    query += "SELECT * FROM SubSection where SubSectionId=" + (documentNoSeriesModel.SubSectionId > 0 ? documentNoSeriesModel.SubSectionId : 0) + ";";
                    query += "SELECT * FROM DocumentProfileNoSeries where ProfileId=" + (documentNoSeriesModel.ProfileID > 0 ? documentNoSeriesModel.ProfileID : 0) + ";";
                    var parameters = new DynamicParameters();
                    parameters.Add("ProfileId", documentNoSeriesModel.ProfileID);
                    parameters.Add("CompanyId", documentNoSeriesModel.PlantID > 0 ? documentNoSeriesModel.PlantID : 0);
                    parameters.Add("DepartmentId", documentNoSeriesModel.DepartmentId > 0 ? documentNoSeriesModel.DepartmentId : 0);
                    parameters.Add("SectionId", documentNoSeriesModel.SectionId > 0 ? documentNoSeriesModel.SectionId : 0);
                    parameters.Add("SubSectionId", documentNoSeriesModel.SubSectionId > 0 ? documentNoSeriesModel.SubSectionId : 0);
                    query += "select  * from ProfileAutoNumber where ProfileId=@ProfileId\n\r";
                    query += "AND\n\r";
                    if (documentNoSeriesModel.PlantID > 0)
                    {
                        query += "CompanyId=@CompanyId\n\r";
                    }
                    else
                    {
                        query += "CompanyId IS NULL\n\r";
                    }
                    query += "AND\n\r";
                    if (documentNoSeriesModel.DepartmentId > 0)
                    {
                        query += "DepartmentId=@DepartmentId\n\r";
                    }
                    else
                    {
                        query += "DepartmentId IS NULL\n\r";
                    }
                    query += "AND\n\r";
                    if (documentNoSeriesModel.SectionId > 0)
                    {
                        query += "SectionId=@SectionId\n\r";
                    }
                    else
                    {
                        query += "SectionId IS NULL\n\r";
                    }
                    query += "AND\n\r";
                    if (documentNoSeriesModel.SubSectionId > 0)
                    {
                        query += "SubSectionId=@SubSectionId\n\r";
                    }
                    else
                    {
                        query += "SubSectionId IS NULL\n\r";
                    }
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    masterLists.Plants = results.Read<Plant>().ToList();
                    masterLists.Departments = results.Read<Department>().ToList();
                    masterLists.Sections = results.Read<Section>().ToList();
                    masterLists.SubSections = results.Read<SubSection>().ToList();
                    masterLists.DocumentProfileNoSeries = results.Read<DocumentProfileNoSeries>().ToList();
                    masterLists.ProfileAutoNumber = results.Read<ProfileAutoNumber>().ToList();
                    return masterLists;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ProfileAutoNumber>> GetProfileAutoNumber(long? Id, DocumentNoSeriesModel documentNoSeriesModel)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ProfileId", Id);
                parameters.Add("CompanyId", documentNoSeriesModel.PlantID > 0 ? documentNoSeriesModel.PlantID : 0);
                parameters.Add("DepartmentId", documentNoSeriesModel.DepartmentId > 0 ? documentNoSeriesModel.DepartmentId : 0);
                parameters.Add("SectionId", documentNoSeriesModel.SectionId > 0 ? documentNoSeriesModel.SectionId : 0);
                parameters.Add("SubSectionId", documentNoSeriesModel.SubSectionId > 0 ? documentNoSeriesModel.SubSectionId : 0);
                var query = "select  * from ProfileAutoNumber where ProfileId=@ProfileId\n\r";
                query += "AND\n\r";
                if (documentNoSeriesModel.PlantID > 0)
                {
                    query += "CompanyId=@CompanyId\n\r";
                }
                else
                {
                    query += "CompanyId IS NULL\n\r";
                }
                query += "AND\n\r";
                if (documentNoSeriesModel.DepartmentId > 0)
                {
                    query += "DepartmentId=@DepartmentId\n\r";
                }
                else
                {
                    query += "DepartmentId IS NULL\n\r";
                }
                query += "AND\n\r";
                if (documentNoSeriesModel.SectionId > 0)
                {
                    query += "SectionId=@SectionId\n\r";
                }
                else
                {
                    query += "SectionId IS NULL\n\r";
                }
                query += "AND\n\r";
                if (documentNoSeriesModel.SubSectionId > 0)
                {
                    query += "SubSectionId=@SubSectionId\n\r";
                }
                else
                {
                    query += "SubSectionId IS NULL\n\r";
                }
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ProfileAutoNumber>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DocumentProfileNoSeries> UpdateDocumentProfileNoSeriesLastCreateDate(DocumentProfileNoSeries profileSettings)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ProfileId", profileSettings.ProfileId);
                        parameters.Add("LastCreatedDate", profileSettings.LastCreatedDate, DbType.DateTime);
                        var query = "Update DocumentProfileNoSeries SET LastCreatedDate=@LastCreatedDate WHERE " +
                            "ProfileId= @ProfileId";
                        await connection.QueryFirstOrDefaultAsync<long>(query, parameters);
                        return profileSettings;
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
        public async Task<ProfileAutoNumber> UpdateDocumentProfileNoSeriesLastNoUsed(ProfileAutoNumber profileAutoNumber)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ProfileAutoNumberId", profileAutoNumber.ProfileAutoNumberId);
                        parameters.Add("LastNoUsed", profileAutoNumber.LastNoUsed);
                        var query = "Update ProfileAutoNumber SET LastNoUsed=@LastNoUsed WHERE " +
                            "ProfileAutoNumberId= @ProfileAutoNumberId";
                        await connection.QueryFirstOrDefaultAsync<long>(query, parameters);
                        return profileAutoNumber;
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
        public async Task<DocumentNoSeries> InsertDocumentNoSeries(DocumentNoSeries documentNoSeries)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ProfileId", documentNoSeries.ProfileId);
                        parameters.Add("DocumentNo", documentNoSeries.DocumentNo, DbType.String);
                        parameters.Add("AddedDate", documentNoSeries.AddedDate, DbType.DateTime);
                        parameters.Add("AddedByUserID", documentNoSeries.AddedByUserId);
                        parameters.Add("StatusCodeId", documentNoSeries.StatusCodeId);
                        parameters.Add("SessionId", documentNoSeries.SessionId, DbType.Guid);
                        parameters.Add("RequestorId", documentNoSeries.RequestorId);
                        parameters.Add("ModifiedDate", documentNoSeries.ModifiedDate);
                        parameters.Add("FileProfileTypeId", documentNoSeries.FileProfileTypeId);
                        parameters.Add("ModifiedByUserId", documentNoSeries.ModifiedByUserId);
                        parameters.Add("IsUpload", documentNoSeries.IsUpload == null ? null : documentNoSeries.IsUpload, (DbType?)SqlDbType.Bit);
                        parameters.Add("VersionNo", documentNoSeries.VersionNo, DbType.String);
                        parameters.Add("EffectiveDate", documentNoSeries.EffectiveDate, DbType.DateTime);
                        parameters.Add("NextReviewDate", documentNoSeries.NextReviewDate, DbType.DateTime);
                        parameters.Add("Date", documentNoSeries.Date, DbType.DateTime);
                        parameters.Add("Link", documentNoSeries.Link, DbType.String);
                        parameters.Add("ReasonToVoid", documentNoSeries.ReasonToVoid, DbType.String);
                        parameters.Add("Title", documentNoSeries.Title, DbType.String);
                        parameters.Add("Description", documentNoSeries.Description, DbType.String);
                        var query = "INSERT INTO [DocumentNoSeries](ProfileId,DocumentNo,AddedDate,AddedByUserID,StatusCodeId," +
                            "SessionId,RequestorId,ModifiedDate,ModifiedByUserId,FileProfileTypeId,IsUpload,VersionNo,EffectiveDate,NextReviewDate,Date,Link,ReasonToVoid,Description,Title) " +
                            "OUTPUT INSERTED.NumberSeriesId VALUES " +
                           "(@ProfileId,@DocumentNo,@AddedDate,@AddedByUserID,@StatusCodeId,@SessionId,@RequestorId," +
                           "@ModifiedDate,@ModifiedByUserId,@FileProfileTypeId,@IsUpload,@VersionNo,@EffectiveDate,@NextReviewDate,@Date,@Link,@ReasonToVoid,@Description,@Title)";
                        documentNoSeries.NumberSeriesId = await connection.QueryFirstOrDefaultAsync<long>(query, parameters);

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

        public async Task<ProfileAutoNumber> InsertProfileAutoNumber(ProfileAutoNumber profileAutoNumber)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ProfileId", profileAutoNumber.ProfileId > 0 ? profileAutoNumber.ProfileId : null);
                        parameters.Add("CompanyId", profileAutoNumber.CompanyId > 0 ? profileAutoNumber.CompanyId : null);
                        parameters.Add("DepartmentId", profileAutoNumber.DepartmentId > 0 ? profileAutoNumber.DepartmentId : null);
                        parameters.Add("SectionId", profileAutoNumber.SectionId > 0 ? profileAutoNumber.SectionId : null);
                        parameters.Add("SubSectionId", profileAutoNumber.SubSectionId > 0 ? profileAutoNumber.SubSectionId : null);
                        parameters.Add("LastNoUsed", profileAutoNumber.LastNoUsed);
                        parameters.Add("ProfileYear", profileAutoNumber.ProfileYear > 0 ? profileAutoNumber.ProfileYear : null);
                        parameters.Add("ScreenId", profileAutoNumber.ScreenId);
                        parameters.Add("ScreenAutoNumberId", profileAutoNumber.ScreenAutoNumberId);

                        var query = "INSERT INTO [ProfileAutoNumber](ProfileId,CompanyId,DepartmentId,SectionId,SubSectionId,LastNoUsed,ProfileYear,ScreenId,ScreenAutoNumberId) " +
                            "OUTPUT INSERTED.ProfileAutoNumberId VALUES " +
                           "(@ProfileId,@CompanyId,@DepartmentId,@SectionId,@SubSectionId,@LastNoUsed,@ProfileYear,@ScreenId,@ScreenAutoNumberId)";
                        profileAutoNumber.ProfileAutoNumberId = await connection.QueryFirstOrDefaultAsync<long>(query, parameters);

                        return profileAutoNumber;
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
