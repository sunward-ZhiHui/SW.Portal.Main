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
        public async Task<MasterListsModel> GetMasterLists()
        {
            try
            {
                MasterListsModel masterLists = new MasterListsModel();
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(@"SELECT * FROM Plant;SELECT * FROM Department;SELECT * FROM Section;SELECT * FROM SubSection;SELECT * FROM DocumentProfileNoSeries;SELECT * FROM DocumentProfileNoSeries;");
                    masterLists.Plants = results.Read<Plant>().ToList();
                    masterLists.Departments = results.Read<Department>().ToList();
                    masterLists.Sections = results.Read<Section>().ToList();
                    masterLists.SubSections = results.Read<SubSection>().ToList();
                    masterLists.DocumentProfileNoSeries = results.Read<DocumentProfileNoSeries>().ToList();
                    return masterLists;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public IReadOnlyList<ProfileAutoNumber> GetProfileAutoNumber(long? Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ProfileId", Id);
                var query = "select  * from ProfileAutoNumber where ProfileId=@ProfileId";
                using (var connection = CreateConnection())
                {
                    return (connection.Query<ProfileAutoNumber>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public DocumentProfileNoSeries UpdateDocumentProfileNoSeriesLastCreateDate(DocumentProfileNoSeries profileSettings)
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
                            parameters.Add("ProfileId", profileSettings.ProfileId);
                            parameters.Add("LastCreatedDate", profileSettings.LastCreatedDate, DbType.DateTime);
                            var query = "Update DocumentProfileNoSeries SET LastCreatedDate=@LastCreatedDate WHERE " +
                                "ProfileId= @ProfileId";
                            connection.QueryFirstOrDefault<long>(query, parameters, transaction);
                            transaction.Commit();
                            return profileSettings;
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
        public ProfileAutoNumber UpdateDocumentProfileNoSeriesLastNoUsed(ProfileAutoNumber profileAutoNumber)
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
                            parameters.Add("ProfileAutoNumberId", profileAutoNumber.ProfileAutoNumberId);
                            parameters.Add("LastNoUsed", profileAutoNumber.LastNoUsed);
                            var query = "Update ProfileAutoNumber SET LastNoUsed=@LastNoUsed WHERE " +
                                "ProfileAutoNumberId= @ProfileAutoNumberId";
                            connection.QueryFirstOrDefault<long>(query, parameters, transaction);
                            transaction.Commit();
                            return profileAutoNumber;
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
        public DocumentNoSeries InsertDocumentNoSeries(DocumentNoSeries documentNoSeries)
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
                            parameters.Add("ProfileId", documentNoSeries.ProfileId);
                            parameters.Add("DocumentNo", documentNoSeries.DocumentNo);
                            parameters.Add("AddedDate", documentNoSeries.AddedDate);
                            parameters.Add("AddedByUserID", documentNoSeries.AddedByUserId);
                            parameters.Add("StatusCodeId", documentNoSeries.StatusCodeId);
                            parameters.Add("SessionId", documentNoSeries.SessionId);
                            parameters.Add("RequestorId", documentNoSeries.RequestorId);
                            parameters.Add("ModifiedDate", documentNoSeries.ModifiedDate);
                            parameters.Add("ModifiedByUserId", documentNoSeries.ModifiedByUserId);

                            var query = "INSERT INTO [DocumentNoSeries](ProfileId,DocumentNo,AddedDate,AddedByUserID,StatusCodeId,SessionId,RequestorId,ModifiedDate,ModifiedByUserId) " +
                                "OUTPUT INSERTED.NumberSeriesId VALUES " +
                               "(@ProfileId,@DocumentNo,@AddedDate,@AddedByUserID,@StatusCodeId,@SessionId,@RequestorId,@ModifiedDate,@ModifiedByUserId)";
                            documentNoSeries.NumberSeriesId = connection.QueryFirstOrDefault<long>(query, parameters, transaction);

                            transaction.Commit();
                            return documentNoSeries;
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

        public ProfileAutoNumber InsertProfileAutoNumber(ProfileAutoNumber profileAutoNumber)
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
                            profileAutoNumber.ProfileAutoNumberId = connection.QueryFirstOrDefault<long>(query, parameters, transaction);

                            transaction.Commit();
                            return profileAutoNumber;
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
