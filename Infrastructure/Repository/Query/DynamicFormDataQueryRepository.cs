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
using Google.Protobuf.Collections;
using static iTextSharp.text.pdf.AcroFields;
using Microsoft.Data.Edm.Values;
using Org.BouncyCastle.Utilities;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Dynamic;

namespace Infrastructure.Repository.Query
{
    public class DynamicFormDataQueryRepository : DbConnector, IDynamicFormDataQueryRepository
    {
        public DynamicFormDataQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        private async Task<DynamicFormDataListItems> GetDynamicFormAllAsync(long? OldDynamicFormId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormID", OldDynamicFormId);
                DynamicFormDataListItems DynamicFormDataListItems = new DynamicFormDataListItems();

                var query = "select t1.* from DynamicForm t1 where (t1.IsDeleted is null OR t1.IsDeleted=0) AND t1.ID=@DynamicFormID;\n";
                query += "select t1.* from DynamicFormSection t1 Where (t1.IsDeleted is null OR t1.IsDeleted=0) AND t1.DynamicFormID IN(select t2.ID from DynamicForm t2 where (t2.IsDeleted is null OR t2.IsDeleted=0) AND t2.ID=@DynamicFormID);\n";
                query += "select t1.* from DynamicFormApproval t1 Where t1.DynamicFormID=@DynamicFormID;\n";
                query += "select t1.* from DynamicFormWorkFlow t1 Where t1.DynamicFormID=@DynamicFormID;\n";
                query += "select t1.* from DynamicFormWorkFlowSection t1 Where t1.DynamicFormWorkFlowID IN (select t2.DynamicFormWorkFlowID from DynamicFormWorkFlow t2 where t2.DynamicFormID=@DynamicFormID);\n";
                query += "select t1.* from DynamicFormWorkFlowApproval t1 where t1.DynamicFormWorkFlowID IN(select t2.DynamicFormWorkFlowID from DynamicFormWorkFlow t2 Where t2.DynamicFormID=@DynamicFormID);\n";

                query += "select t1.* from DynamicFormSectionAttribute t1 Where (t1.IsDeleted is null OR t1.IsDeleted=0) AND t1.DynamicFormSectionID IN (select t2.DynamicFormSectionID from DynamicFormSection t2 Where (t2.IsDeleted is null OR t2.IsDeleted=0) AND t2.DynamicFormID=@DynamicFormID);\n";
                query += "select t1.* from DynamicFormSectionAttributeSectionParent t1 where t1.DynamicFormSectionAttributeID IN(select t2.DynamicFormSectionAttributeID from DynamicFormSectionAttribute t2 Where (t2.IsDeleted is null OR t2.IsDeleted=0) AND t2.DynamicFormSectionID IN (select t3.DynamicFormSectionID from DynamicFormSection t3 Where (t3.IsDeleted is null OR t3.IsDeleted=0) AND t3.DynamicFormID=@DynamicFormID));\n";
                query += "select t1.* from DynamicFormSectionAttributeSection t1 where t1.DynamicFormSectionAttributeSectionParentID IN \r\n(select t2.DynamicFormSectionAttributeSectionParentID from DynamicFormSectionAttributeSectionParent t2 where  t2.DynamicFormSectionAttributeID IN\r\n(select t3.DynamicFormSectionAttributeID from DynamicFormSectionAttribute t3 Where (t3.IsDeleted is null OR t3.IsDeleted=0) AND t3.DynamicFormSectionID IN(Select t4.DynamicFormSectionID from DynamicFormSection t4 where (t4.IsDeleted is null OR t4.IsDeleted=0) AND t4.DynamicFormID= @DynamicFormID)));\n";
                query += "select t1.* from DynamicFormSectionSecurity t1 where DynamicFormSectionID IN(select t2.DynamicFormSectionID from DynamicFormSection t2 Where (t2.IsDeleted is null OR t2.IsDeleted=0) AND t2.DynamicFormID=@DynamicFormID);\n";
                query += "select t1.* from DynamicFormSectionAttributeSecurity t1 where t1.DynamicFormSectionAttributeID IN(select t2.DynamicFormSectionAttributeID from DynamicFormSectionAttribute t2 Where (t2.IsDeleted is null OR t2.IsDeleted=0) AND  t2.DynamicFormSectionID IN (select t3.DynamicFormSectionID from DynamicFormSection t3 Where t3.DynamicFormID=@DynamicFormID));\n";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    DynamicFormDataListItems.DynamicForm = results.ReadAsync<DynamicForm>().Result.FirstOrDefault();
                    if (DynamicFormDataListItems.DynamicForm != null)
                    {
                        DynamicFormDataListItems.DynamicFormSection = results.ReadAsync<DynamicFormSection>().Result.ToList();
                        DynamicFormDataListItems.DynamicFormApproval = results.ReadAsync<DynamicFormApproval>().Result.ToList();
                        DynamicFormDataListItems.DynamicFormWorkFlow = results.ReadAsync<DynamicFormWorkFlow>().Result.ToList();
                        DynamicFormDataListItems.DynamicFormWorkFlowSection = results.ReadAsync<DynamicFormWorkFlowSection>().Result.ToList();
                        DynamicFormDataListItems.DynamicFormWorkFlowApproval = results.ReadAsync<DynamicFormWorkFlowApproval>().Result.ToList();
                        if (DynamicFormDataListItems.DynamicFormSection != null && DynamicFormDataListItems.DynamicFormSection.Count() > 0)
                        {
                            DynamicFormDataListItems.DynamicFormSectionAttribute = results.ReadAsync<DynamicFormSectionAttribute>().Result.ToList();
                            DynamicFormDataListItems.DynamicFormSectionAttributeSectionParent = results.ReadAsync<DynamicFormSectionAttributeSectionParent>().Result.ToList();
                            DynamicFormDataListItems.DynamicFormSectionAttributeSection = results.ReadAsync<DynamicFormSectionAttributeSection>().Result.ToList();
                            DynamicFormDataListItems.DynamicFormSectionSecurity = results.ReadAsync<DynamicFormSectionSecurity>().Result.ToList();
                            DynamicFormDataListItems.DynamicFormSectionAttributeSecurity = results.ReadAsync<DynamicFormSectionAttributeSecurity>().Result.ToList();
                        }
                    }
                }
                return DynamicFormDataListItems;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicForm> InsertCloneDynamicForm(DynamicForm dynamicForms, bool? IsWithoutForm, long? UserId)
        {
            long NewDynamicFormId = 0;
            try
            {
                DateTime DateTime = DateTime.Now;
                var result = await GetDynamicFormAllAsync(dynamicForms.OldDynamicFormId);
                if (result != null && result.DynamicForm != null && result.DynamicForm.ID > 0)
                {
                    using (var connection = CreateConnection())
                    {
                        try
                        {
                            var dynamicForm = result.DynamicForm;
                            var parameters = new DynamicParameters();
                            parameters.Add("Name", dynamicForms.Name, DbType.String);
                            parameters.Add("ScreenID", dynamicForms.ScreenID, DbType.String);
                            parameters.Add("SessionID", dynamicForms.SessionId, DbType.Guid);
                            parameters.Add("AttributeID", dynamicForms.AttributeID);
                            parameters.Add("AddedByUserID", UserId);
                            parameters.Add("ModifiedByUserID", UserId);
                            parameters.Add("AddedDate", DateTime.Now, DbType.DateTime);
                            parameters.Add("ModifiedDate", DateTime.Now, DbType.DateTime);
                            parameters.Add("StatusCodeID", dynamicForms.StatusCodeID);
                            parameters.Add("IsApproval", dynamicForms.IsApproval);
                            parameters.Add("FileProfileTypeId", dynamicForms.FileProfileTypeId);
                            parameters.Add("IsUpload", dynamicForms.IsUpload);
                            parameters.Add("CompanyId", dynamicForms.CompanyId);
                            parameters.Add("ProfileId", dynamicForms.ProfileId);
                            parameters.Add("IsGridForm", dynamicForms.IsGridForm);
                            var query = "INSERT INTO DynamicForm(ScreenID,IsGridForm,Name,SessionID,AttributeID,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,IsApproval,FileProfileTypeId,IsUpload,CompanyId,ProfileId) OUTPUT INSERTED.ID VALUES " +
                                "(@ScreenID,@IsGridForm,@Name,@SessionID,@AttributeID,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@IsApproval,@FileProfileTypeId,@IsUpload,@CompanyId,@ProfileId)";
                            NewDynamicFormId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                            result.DynamicForm.CloneDynamicFormId = NewDynamicFormId;
                            dynamicForms.CloneDynamicFormId = NewDynamicFormId;
                            result.DynamicForm.SessionID = (Guid)dynamicForms.SessionId;
                        }
                        catch (Exception exp)
                        {
                            await Delete(NewDynamicFormId);
                            throw new Exception(exp.Message, exp);
                        }
                    }
                    if (NewDynamicFormId > 0)
                    {
                        await InsertOrUpdateDynamicFormSection(result, NewDynamicFormId, UserId, DateTime);
                        await InsertOrUpdateDynamicFormApproval(result, NewDynamicFormId, UserId, DateTime);
                        await InsertDynamicFormWorkFlow(result, NewDynamicFormId);
                        await InsertOrUpdateDynamicFormSectionAttribute(result, UserId, DateTime, NewDynamicFormId);
                        if (IsWithoutForm == true)
                        {
                            await GetDynamicFormData(result, dynamicForms, NewDynamicFormId);
                            await InsertOrUpdateDynamicFormData(result, UserId, DateTime, NewDynamicFormId);
                            await InsertDynamicFormDataSectionLock(result, NewDynamicFormId, "", 0);
                            await InsertDynamicFormApproved(result, NewDynamicFormId, "", 0);
                            await InsertDynamicFormWorkFlowForm(result, NewDynamicFormId, "", 0);
                            await InsertDynamicFormDataGrid(result, UserId, DateTime, NewDynamicFormId);
                        }
                    }
                }
                return dynamicForms;

            }
            catch (Exception exp)
            {
                await Delete(NewDynamicFormId);
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormDataListItems> InsertOrUpdateDynamicFormSection(DynamicFormDataListItems result, long DynamicFormId, long? UserId, DateTime DateTime)
        {
            try
            {
                if (result.DynamicFormSection != null && result.DynamicFormSection.Count() > 0)
                {
                    using (var connection = CreateConnection())
                    {
                        try
                        {
                            var querys = string.Empty;
                            foreach (var dynamicFormSection in result.DynamicFormSection)
                            {
                                var parameters = new DynamicParameters();
                                parameters.Add("SectionName", dynamicFormSection.SectionName, DbType.String);
                                parameters.Add("DynamicFormId", DynamicFormId);
                                parameters.Add("SortOrderBy", dynamicFormSection.SortOrderBy);
                                parameters.Add("SessionId", Guid.NewGuid(), DbType.Guid);
                                parameters.Add("AddedByUserID", UserId);
                                parameters.Add("ModifiedByUserID", UserId);
                                parameters.Add("AddedDate", DateTime, DbType.DateTime);
                                parameters.Add("ModifiedDate", DateTime, DbType.DateTime);
                                parameters.Add("StatusCodeID", dynamicFormSection.StatusCodeID);
                                parameters.Add("IsAutoNumberEnabled", dynamicFormSection.IsAutoNumberEnabled == true ? true : null);
                                parameters.Add("IsVisible", dynamicFormSection.IsVisible);
                                parameters.Add("IsReadOnly", dynamicFormSection.IsReadOnly);
                                parameters.Add("IsReadWrite", dynamicFormSection.IsReadWrite);
                                parameters.Add("Instruction", dynamicFormSection.Instruction, DbType.String);
                                parameters.Add("SectionFileProfileTypeId", dynamicFormSection.SectionFileProfileTypeId);
                                var query = "INSERT INTO DynamicFormSection(IsAutoNumberEnabled,SectionFileProfileTypeId,SectionName,DynamicFormId,SessionId,SortOrderBy,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,IsVisible,IsReadOnly,IsReadWrite,Instruction)  " +
                                    "OUTPUT INSERTED.DynamicFormSectionId VALUES " +
                                    "(@IsAutoNumberEnabled,@SectionFileProfileTypeId,@SectionName,@DynamicFormId,@SessionId,@SortOrderBy,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@IsVisible,@IsReadOnly,@IsReadWrite,@Instruction)";
                                dynamicFormSection.DynamicFormCloneSectionId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);

                                if (result.DynamicFormSectionSecurity != null && result.DynamicFormSectionSecurity.Count() > 0)
                                {
                                    var oldItems = result.DynamicFormSectionSecurity.Where(w => w.DynamicFormSectionId == dynamicFormSection.DynamicFormSectionId).ToList();
                                    if (oldItems != null && oldItems.Count() > 0)
                                    {
                                        foreach (var items in oldItems)
                                        {
                                            if (items.UserId > 0)
                                            {
                                                string? UserGroupId = items.UserGroupId > 0 ? items.UserId.ToString() : "null";
                                                string? levelId = items.LevelId > 0 ? items.LevelId.ToString() : "null";
                                                querys += "INSERT INTO [DynamicFormSectionSecurity](DynamicFormSectionID,UserId,UserGroupID,LevelID,IsReadWrite,IsReadOnly,IsVisible) OUTPUT INSERTED.DynamicFormSectionSecurityID " +
                                           "VALUES (" + dynamicFormSection.DynamicFormCloneSectionId + "," + items.UserId + "," + UserGroupId + "," + levelId + ",'" + items.IsReadWrite + "','" + items.IsReadOnly + "','" + items.IsVisible + "'" + ");\n";
                                            }
                                        }
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(querys))
                            {
                                await connection.QuerySingleOrDefaultAsync<long>(querys);
                            }
                        }
                        catch (Exception exp)
                        {
                            await Delete(DynamicFormId);
                            throw new Exception(exp.Message, exp);
                        }

                    }
                }
                return result;
            }
            catch (Exception exp)
            {
                await Delete(DynamicFormId);
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormDataListItems> InsertOrUpdateDynamicFormApproval(DynamicFormDataListItems result, long DynamicFormId, long? UserId, DateTime DateTime)
        {
            try
            {
                if (result.DynamicFormApproval != null && result.DynamicFormSection.Count() > 0)
                {
                    using (var connection = CreateConnection())
                    {
                        try
                        {
                            foreach (var dynamicFormApproval in result.DynamicFormApproval)
                            {
                                var parameters = new DynamicParameters();
                                parameters.Add("DynamicFormApprovalId", dynamicFormApproval.DynamicFormApprovalId);
                                parameters.Add("ApprovalUserId", dynamicFormApproval.ApprovalUserId);
                                parameters.Add("DynamicFormId", DynamicFormId);
                                parameters.Add("AddedByUserID", UserId);
                                parameters.Add("ModifiedByUserID", UserId);
                                parameters.Add("AddedDate", DateTime, DbType.DateTime);
                                parameters.Add("ModifiedDate", DateTime, DbType.DateTime);
                                parameters.Add("StatusCodeID", dynamicFormApproval.StatusCodeID);
                                parameters.Add("IsApproved", dynamicFormApproval.IsApproved);
                                parameters.Add("SortOrderBy", dynamicFormApproval.SortOrderBy);
                                parameters.Add("Description", dynamicFormApproval.Description, DbType.String);
                                var query = "INSERT INTO DynamicFormApproval(ApprovalUserId,DynamicFormId,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,SortOrderBy,IsApproved,Description)  OUTPUT INSERTED.DynamicFormApprovalId VALUES " +
                                    "(@ApprovalUserId,@DynamicFormId,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@SortOrderBy,@IsApproved,@Description);\n\r";
                                dynamicFormApproval.DynamicFormApprovalId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                            }
                        }
                        catch (Exception exp)
                        {
                            await Delete(DynamicFormId);
                            throw new Exception(exp.Message, exp);
                        }
                    }
                }
                return result;
            }
            catch (Exception exp)
            {
                await Delete(DynamicFormId);
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormDataListItems> InsertDynamicFormWorkFlow(DynamicFormDataListItems result, long DynamicFormId)
        {
            try
            {
                if (result.DynamicFormWorkFlow != null && result.DynamicFormWorkFlow.Count() > 0)
                {
                    using (var connection = CreateConnection())
                    {
                        try
                        {
                            foreach (var value in result.DynamicFormWorkFlow)
                            {
                                var query = string.Empty;
                                var parameters = new DynamicParameters();
                                parameters.Add("DynamicFormId", DynamicFormId);
                                parameters.Add("Type", "User");
                                parameters.Add("SequenceNo", value.SequenceNo);
                                parameters.Add("UserId", value.UserId);
                                parameters.Add("IsAllowDelegateUser", value.IsAllowDelegateUser == true ? true : null);
                                query = "INSERT INTO [DynamicFormWorkFlow](DynamicFormId,UserId,Type,SequenceNo,IsAllowDelegateUser) OUTPUT INSERTED.DynamicFormWorkFlowId " +
                                                   "VALUES (@DynamicFormId,@UserId,@Type,@SequenceNo,@IsAllowDelegateUser);\n";
                                var dynamicFormWorkFlowId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                                var querys = string.Empty;
                                if (result.DynamicFormWorkFlowSection != null && result.DynamicFormWorkFlowSection.Count() > 0)
                                {
                                    var oldItems = result.DynamicFormWorkFlowSection.Where(w => w.DynamicFormWorkFlowId == value.DynamicFormWorkFlowId).ToList();
                                    if (oldItems != null && oldItems.Count() > 0)
                                    {
                                        foreach (var items in oldItems)
                                        {
                                            if (items.DynamicFormSectionId > 0)
                                            {
                                                var newDynamicFormSectionId = result.DynamicFormSection.FirstOrDefault(f => f.DynamicFormSectionId == items.DynamicFormSectionId)?.DynamicFormCloneSectionId;
                                                if (newDynamicFormSectionId > 0)
                                                {
                                                    querys += "INSERT INTO [DynamicFormWorkFlowSection](DynamicFormWorkFlowId,DynamicFormSectionId) OUTPUT INSERTED.DynamicFormWorkFlowSectionID " +
                                               "VALUES (" + dynamicFormWorkFlowId + "," + newDynamicFormSectionId + ");\n";
                                                }
                                            }
                                        }
                                    }

                                }
                                if (result.DynamicFormWorkFlowApproval != null && result.DynamicFormWorkFlowApproval.Count() > 0)
                                {
                                    var oldIApptems = result.DynamicFormWorkFlowApproval.Where(w => w.DynamicFormWorkFlowId == value.DynamicFormWorkFlowId).ToList();
                                    if (oldIApptems != null && oldIApptems.Count() > 0)
                                    {
                                        foreach (var items in oldIApptems)
                                        {
                                            if (items.UserId > 0)
                                            {
                                                string? SortBy = items.SortBy > 0 ? items.SortBy.ToString() : "null";
                                                querys += "INSERT INTO [DynamicFormWorkFlowApproval](DynamicFormWorkFlowId,UserId,SortBy) OUTPUT INSERTED.DynamicFormWorkFlowApprovalId " +
                                           "VALUES (" + dynamicFormWorkFlowId + "," + items.UserId + "," + SortBy + ");\n";
                                            }
                                        }
                                    }

                                }
                                if (!string.IsNullOrEmpty(querys))
                                {
                                    await connection.QuerySingleOrDefaultAsync<long>(querys);
                                }
                            }
                        }
                        catch (Exception exp)
                        {
                            await Delete(DynamicFormId);
                            throw new Exception(exp.Message, exp);
                        }
                    }
                }
                return result;
            }
            catch (Exception exp)
            {
                await Delete(DynamicFormId);
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormDataListItems> InsertOrUpdateDynamicFormSectionAttribute(DynamicFormDataListItems result, long? UserId, DateTime DateTime, long DynamicFormId)
        {
            try
            {
                if (result.DynamicFormSectionAttribute != null && result.DynamicFormSectionAttribute.Count() > 0)
                {
                    using (var connection = CreateConnection())
                    {
                        try
                        {
                            int i = 0;
                            foreach (var dynamicFormSection in result.DynamicFormSectionAttribute)
                            {
                                var newDynamicFormSectionId = result.DynamicFormSection.FirstOrDefault(f => f.DynamicFormSectionId == dynamicFormSection.DynamicFormSectionId)?.DynamicFormCloneSectionId;
                                if (newDynamicFormSectionId > 0)
                                {
                                    var parameters = new DynamicParameters();
                                    parameters.Add("DynamicFormSectionId", newDynamicFormSectionId);
                                    parameters.Add("DisplayName", dynamicFormSection.DisplayName, DbType.String);
                                    parameters.Add("AttributeId", dynamicFormSection.AttributeId);
                                    parameters.Add("SortOrderBy", dynamicFormSection.SortOrderBy);
                                    parameters.Add("SessionId", Guid.NewGuid(), DbType.Guid);
                                    parameters.Add("AddedByUserID", UserId);
                                    parameters.Add("ModifiedByUserID", UserId);
                                    parameters.Add("AddedDate", DateTime, DbType.DateTime);
                                    parameters.Add("ModifiedDate", DateTime, DbType.DateTime);
                                    parameters.Add("StatusCodeID", dynamicFormSection.StatusCodeID);
                                    parameters.Add("ColSpan", dynamicFormSection.ColSpan);
                                    parameters.Add("IsRequired", dynamicFormSection.IsRequired);
                                    parameters.Add("IsMultiple", dynamicFormSection.IsMultiple);
                                    parameters.Add("RequiredMessage", dynamicFormSection.RequiredMessage, DbType.String);
                                    parameters.Add("IsSpinEditType", dynamicFormSection.IsSpinEditType, DbType.String);
                                    parameters.Add("IsDisplayTableHeader", dynamicFormSection.IsDisplayTableHeader);
                                    parameters.Add("FormToolTips", dynamicFormSection.FormToolTips, DbType.String);
                                    parameters.Add("IsVisible", dynamicFormSection.IsVisible);
                                    parameters.Add("IsRadioCheckRemarks", dynamicFormSection.IsRadioCheckRemarks, DbType.String);
                                    parameters.Add("RadioLayout", dynamicFormSection.RadioLayout, DbType.String);
                                    parameters.Add("RemarksLabelName", dynamicFormSection.RemarksLabelName, DbType.String);
                                    parameters.Add("PlantDropDownWithOtherDataSourceId", dynamicFormSection.PlantDropDownWithOtherDataSourceId);
                                    parameters.Add("PlantDropDownWithOtherDataSourceLabelName", dynamicFormSection.PlantDropDownWithOtherDataSourceLabelName, DbType.String);
                                    parameters.Add("IsPlantLoadDependency", dynamicFormSection.IsPlantLoadDependency == true ? true : null);
                                    parameters.Add("IsDefaultReadOnly", dynamicFormSection.IsDefaultReadOnly == true ? true : null);
                                    parameters.Add("IsSetDefaultValue", dynamicFormSection.IsSetDefaultValue == true ? true : null);
                                    parameters.Add("IsDependencyMultiple", dynamicFormSection.IsDependencyMultiple == true ? true : null);
                                    parameters.Add("IsDisplayDropDownHeader", dynamicFormSection.IsDisplayDropDownHeader);
                                    parameters.Add("IsDynamicFormGridDropdown", dynamicFormSection.IsDynamicFormGridDropdown == true ? true : null);
                                    parameters.Add("GridDropDownDynamicFormID", dynamicFormSection.GridDropDownDynamicFormID);
                                    parameters.Add("IsDynamicFormGridDropdownMultiple", dynamicFormSection.IsDynamicFormGridDropdownMultiple == true ? true : null);
                                    parameters.Add("ApplicationMasterIds", dynamicFormSection.ApplicationMasterIds, DbType.String);
                                    parameters.Add("PlantDropDownWithOtherDataSourceIds", dynamicFormSection.PlantDropDownWithOtherDataSourceIds, DbType.String);

                                    var query = "INSERT INTO DynamicFormSectionAttribute(IsDynamicFormGridDropdownMultiple,IsDynamicFormGridDropdown,GridDropDownDynamicFormID,IsDependencyMultiple,IsDisplayDropDownHeader,ApplicationMasterIds,IsSetDefaultValue,IsDefaultReadOnly,PlantDropDownWithOtherDataSourceIds,IsPlantLoadDependency,PlantDropDownWithOtherDataSourceLabelName,PlantDropDownWithOtherDataSourceId,RemarksLabelName,IsRadioCheckRemarks,RadioLayout,FormToolTips,DisplayName,AttributeId,SessionId,SortOrderBy,AddedByUserID," +
                                        "ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,ColSpan,DynamicFormSectionId,IsRequired,IsMultiple,RequiredMessage,IsSpinEditType,IsDisplayTableHeader,IsVisible) OUTPUT INSERTED.DynamicFormSectionAttributeId VALUES " +
                                        "(@IsDynamicFormGridDropdownMultiple,@IsDynamicFormGridDropdown,@GridDropDownDynamicFormID,@IsDependencyMultiple,@IsDisplayDropDownHeader,@ApplicationMasterIds,@IsSetDefaultValue,@IsDefaultReadOnly,@PlantDropDownWithOtherDataSourceIds,@IsPlantLoadDependency,@PlantDropDownWithOtherDataSourceLabelName,@PlantDropDownWithOtherDataSourceId,@RemarksLabelName,@IsRadioCheckRemarks,@RadioLayout,@FormToolTips,@DisplayName,@AttributeId,@SessionId,@SortOrderBy," +
                                        "@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@ColSpan,@DynamicFormSectionId,@IsRequired,@IsMultiple,@RequiredMessage,@IsSpinEditType,@IsDisplayTableHeader,@IsVisible)";

                                    var NewdynamicFormSectionAttributeId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                                    result.DynamicFormSectionAttribute[i].NewDynamicFormSectionAttributeId = NewdynamicFormSectionAttributeId;
                                    var queryss = string.Empty;
                                    if (result.DynamicFormSectionAttributeSecurity != null && result.DynamicFormSectionAttributeSecurity.Count() > 0)
                                    {
                                        var oldIApptems = result.DynamicFormSectionAttributeSecurity.Where(w => w.DynamicFormSectionAttributeId == dynamicFormSection.DynamicFormSectionAttributeId).ToList();
                                        if (oldIApptems != null && oldIApptems.Count() > 0)
                                        {
                                            foreach (var dynamicFormSectionAttributeSecurity in oldIApptems)
                                            {
                                                if (dynamicFormSectionAttributeSecurity.UserId > 0)
                                                {
                                                    if (dynamicFormSectionAttributeSecurity.UserId > 0)
                                                    {
                                                        string? UserGroupId = dynamicFormSectionAttributeSecurity.UserGroupId > 0 ? dynamicFormSectionAttributeSecurity.UserId.ToString() : "null";
                                                        string? levelId = dynamicFormSectionAttributeSecurity.LevelId > 0 ? dynamicFormSectionAttributeSecurity.LevelId.ToString() : "null";
                                                        queryss += "INSERT INTO [DynamicFormSectionAttributeSecurity](DynamicFormSectionAttributeID,UserId,UserGroupID,LevelID,IsAccess,IsViewFormatOnly,UserType) OUTPUT INSERTED.DynamicFormSectionAttributeSecurityId " +
                                                   "VALUES (" + NewdynamicFormSectionAttributeId + "," + dynamicFormSectionAttributeSecurity.UserId + "," + UserGroupId + "," + levelId + ",'" + dynamicFormSectionAttributeSecurity.IsAccess + "','" + dynamicFormSectionAttributeSecurity.IsViewFormatOnly + "','" + dynamicFormSectionAttributeSecurity.UserType + "'" + ");\n";
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (result.DynamicFormSectionAttributeSectionParent != null && result.DynamicFormSectionAttributeSectionParent.Count() > 0)
                                    {
                                        var oldISectems = result.DynamicFormSectionAttributeSectionParent.Where(w => w.DynamicFormSectionAttributeId == dynamicFormSection.DynamicFormSectionAttributeId).ToList();
                                        if (oldISectems != null && oldISectems.Count() > 0)
                                        {
                                            foreach (var dynamicFormSectionAttributeSectionParent in oldISectems)
                                            {
                                                var parameters1 = new DynamicParameters();
                                                parameters1.Add("DynamicFormSectionAttributeId", NewdynamicFormSectionAttributeId);
                                                parameters1.Add("SequenceNo", dynamicFormSectionAttributeSectionParent.SequenceNo);
                                                var querys = "INSERT INTO DynamicFormSectionAttributeSectionParent(DynamicFormSectionAttributeId,SequenceNo) OUTPUT INSERTED.DynamicFormSectionAttributeSectionParentId VALUES " +
                                                    "(@DynamicFormSectionAttributeId,@SequenceNo);\n\r";
                                                var NewDynamicFormSectionAttributeSectionParentId = await connection.QuerySingleOrDefaultAsync<long>(querys, parameters1);

                                                if (result.DynamicFormSectionAttributeSection != null && result.DynamicFormSectionAttributeSection.Count() > 0)
                                                {
                                                    var oldIPartems = result.DynamicFormSectionAttributeSection.Where(w => w.DynamicFormSectionAttributeSectionParentId == dynamicFormSectionAttributeSectionParent.DynamicFormSectionAttributeSectionParentId).ToList();
                                                    if (oldIPartems != null && oldIPartems.Count() > 0)
                                                    {
                                                        foreach (var items in oldIPartems)
                                                        {
                                                            var newDynamicFormSectionIds = result.DynamicFormSection.FirstOrDefault(f => f.DynamicFormSectionId == items.DynamicFormSectionId)?.DynamicFormCloneSectionId;
                                                            if (newDynamicFormSectionIds > 0)
                                                            {
                                                                queryss += "INSERT INTO DynamicFormSectionAttributeSection(DynamicFormSectionAttributeSectionParentId,DynamicFormSectionID,DynamicFormSectionSelectionByID,DynamicFormSectionSelectionID) VALUES " +
                                                        "(" + NewDynamicFormSectionAttributeSectionParentId + "," + newDynamicFormSectionIds + ",'" + items.DynamicFormSectionSelectionById + "'," + items.DynamicFormSectionSelectionId + ");\n\r";
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(queryss))
                                    {
                                        await connection.QuerySingleOrDefaultAsync<long>(queryss);
                                    }
                                    i++;
                                }
                            }
                        }
                        catch (Exception exp)
                        {
                            await Delete(DynamicFormId);
                            throw new Exception(exp.Message, exp);
                        }

                    }

                }
                return result;
            }
            catch (Exception exp)
            {
                await Delete(DynamicFormId);
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<DynamicFormDataListItems> GetDynamicFormData(DynamicFormDataListItems result, DynamicForm dynamicForm, long DynamicFormId)
        {
            try
            {
                var resultData = new List<DynamicFormData>();
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormId", dynamicForm.OldDynamicFormId);
                query = "SELECT * FROM DynamicFormData Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormId = @DynamicFormId;";
                using (var connection = CreateConnection())
                {
                    resultData = (await connection.QueryAsync<DynamicFormData>(query, parameters)).ToList();
                }
                if (resultData != null && resultData.Count() > 0)
                {
                    result.DynamicFormData = resultData != null ? resultData : new List<DynamicFormData>();
                    var DynamicFormDataIds = string.Join(',', resultData.Select(s => s.DynamicFormDataId).ToList());
                    var query1 = string.Empty;
                    query1 += "select t1.* from DynamicFormData t1 where (t1.IsDeleted=0 or t1.IsDeleted is null) AND t1.DynamicFormDataGridID IN(" + DynamicFormDataIds + ");\n";
                    query1 += "select t1.* from DynamicFormApproved t1 where t1.DynamicFormDataID IN(" + DynamicFormDataIds + ");\n";
                    query1 += "select t1.* from DynamicFormApprovedChanged t1 Where t1.DynamicFormApprovedID IN(select t2.DynamicFormApprovedID from DynamicFormApproved t2 where t2.DynamicFormDataID IN(" + DynamicFormDataIds + "));\n";
                    query1 += "select t1.* from DynamicFormDataSectionLock t1 where t1.DynamicFormDataID IN(" + DynamicFormDataIds + ");\n";
                    query1 += "select t1.* from DynamicFormWorkFlowForm t1 where t1.DynamicFormDataID IN(" + DynamicFormDataIds + ");\n";
                    query1 += "select t1.* from DynamicFormWorkFlowSectionForm t1 Where t1.DynamicFormWorkFlowFormID IN(select t2.DynamicFormWorkFlowFormID from DynamicFormWorkFlowForm t2 Where t2.DynamicFormDataID IN(" + DynamicFormDataIds + "));\n";
                    query1 += "select t1.* from DynamicFormWorkFlowFormDelegate t1 Where t1.DynamicFormWorkFlowFormID IN(select t2.DynamicFormWorkFlowFormID from DynamicFormWorkFlowForm t2 Where t2.DynamicFormDataID IN(" + DynamicFormDataIds + "));\n";
                    query1 += "select t1.* from DynamicFormWorkFlowApprovedForm t1 Where t1.DynamicFormWorkFlowFormID IN(select t2.DynamicFormWorkFlowFormID from DynamicFormWorkFlowForm t2 Where t2.DynamicFormDataID IN(" + DynamicFormDataIds + "));\n";
                    query1 += "select t1.* from DynamicFormWorkFlowApprovedFormChanged t1 Where t1.DynamicFormWorkFlowApprovedFormID IN(select t2.DynamicFormWorkFlowApprovedFormID from DynamicFormWorkFlowApprovedForm t2 Where t2.DynamicFormWorkFlowFormID IN(select t3.DynamicFormWorkFlowFormID from DynamicFormWorkFlowForm t3 Where t3.DynamicFormDataID IN(" + DynamicFormDataIds + ")));\n";
                    using (var connection = CreateConnection())
                    {
                        var results = await connection.QueryMultipleAsync(query1);
                        result.DynamicFormDataGrid = results.ReadAsync<DynamicFormData>().Result.ToList();
                        result.DynamicFormApproved = results.ReadAsync<DynamicFormApproved>().Result.ToList();
                        result.DynamicFormApprovedChanged = results.ReadAsync<DynamicFormApprovedChanged>().Result.ToList();
                        result.DynamicFormDataSectionLock = results.ReadAsync<DynamicFormDataSectionLock>().Result.ToList();
                        result.DynamicFormWorkFlowForm = results.ReadAsync<DynamicFormWorkFlowForm>().Result.ToList();
                        result.DynamicFormWorkFlowSectionForm = results.ReadAsync<DynamicFormWorkFlowSectionForm>().Result.ToList();
                        result.DynamicFormWorkFlowFormDelegate = results.ReadAsync<DynamicFormWorkFlowFormDelegate>().Result.ToList();
                        result.DynamicFormWorkFlowApprovedForm = results.ReadAsync<DynamicFormWorkFlowApprovedForm>().Result.ToList();
                        result.DynamicFormWorkFlowApprovedFormChanged = results.ReadAsync<DynamicFormWorkFlowApprovedFormChanged>().Result.ToList();
                    }
                }
                return result;
            }
            catch (Exception exp)
            {
                await Delete(DynamicFormId);
                throw new Exception(exp.Message, exp);
            }
        }
        private static bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public async Task<DynamicFormDataListItems> InsertOrUpdateDynamicFormData(DynamicFormDataListItems result, long? UserId, DateTime DateTime, long DynamicFormId)
        {
            try
            {
                if (result.DynamicFormData != null && result.DynamicFormData.Count() > 0)
                {
                    using (var connection = CreateConnection())
                    {
                        try
                        {
                            int i = 0;
                            foreach (var dynamicFormData in result.DynamicFormData)
                            {
                                
                                long? NewDynamicFormSectionGridAttributeId = null;
                                if (dynamicFormData.DynamicFormSectionGridAttributeId > 0)
                                {
                                    NewDynamicFormSectionGridAttributeId = result.DynamicFormSectionAttribute.FirstOrDefault(f => f.DynamicFormSectionAttributeId == dynamicFormData.DynamicFormSectionGridAttributeId)?.NewDynamicFormSectionAttributeId;
                                }
                                var parameters = new DynamicParameters();
                                IDictionary<string, object> objectDataList = new ExpandoObject();
                                dynamic jsonObj = new object();
                                if (dynamicFormData.DynamicFormItem != null && IsValidJson(dynamicFormData.DynamicFormItem))
                                {
                                    jsonObj = JsonConvert.DeserializeObject(dynamicFormData.DynamicFormItem);
                                    ExpandoObject listData = jsonObj.ToObject<ExpandoObject>();
                                    var li = listData.ToList();
                                    if (li != null && li.Count() > 0)
                                    {
                                        li.ForEach(f =>
                                        {
                                            var keyData = f.Key;
                                            var valueData = f.Value;
                                            var keyDatas = keyData.Split("_").ToList();
                                            if (keyDatas != null && keyDatas.Count() > 0)
                                            {
                                                var key = keyDatas[0];
                                                var keyCount = key.Count();
                                                var aa = keyData.Substring(keyCount, keyData.Length - keyCount);
                                                var dId = result.DynamicFormSectionAttribute.FirstOrDefault(r => r.DynamicFormSectionAttributeId == long.Parse(key))?.NewDynamicFormSectionAttributeId;
                                                if (dId > 0)
                                                {
                                                    string setData = dId + aa;
                                                    objectDataList[setData] = valueData;
                                                }
                                            }

                                        });
                                    }
                                }
                                parameters.Add("DynamicFormItem", JsonConvert.SerializeObject(objectDataList), DbType.String);
                                parameters.Add("DynamicFormId", DynamicFormId);
                                parameters.Add("SessionId", Guid.NewGuid(), DbType.Guid);
                                parameters.Add("AddedByUserID", UserId);
                                parameters.Add("ModifiedByUserID", UserId);
                                parameters.Add("AddedDate", DateTime, DbType.DateTime);
                                parameters.Add("ModifiedDate", DateTime, DbType.DateTime);
                                parameters.Add("StatusCodeID", dynamicFormData.StatusCodeID);
                                parameters.Add("IsSendApproval", dynamicFormData.IsSendApproval);
                                parameters.Add("FileProfileSessionID", dynamicFormData.FileProfileSessionID, DbType.Guid);
                                parameters.Add("DynamicFormDataGridId", dynamicFormData.DynamicFormDataGridId);
                                parameters.Add("DynamicFormSectionGridAttributeId", NewDynamicFormSectionGridAttributeId);
                                parameters.Add("ProfileId", dynamicFormData.ProfileId);
                                parameters.Add("ProfileNo", dynamicFormData.ProfileNo, DbType.String);
                                parameters.Add("SortOrderByNo", dynamicFormData.SortOrderByNo);
                                parameters.Add("GridSortOrderByNo", dynamicFormData.GridSortOrderByNo);
                                parameters.Add("IsLocked", dynamicFormData.IsLocked);
                                parameters.Add("LockedUserId", dynamicFormData.LockedUserId);
                                var query = "INSERT INTO DynamicFormData(LockedUserId,IsLocked,DynamicFormSectionGridAttributeId,GridSortOrderByNo,SortOrderByNo,DynamicFormDataGridId,DynamicFormItem,DynamicFormId,SessionId,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,IsSendApproval,FileProfileSessionID,ProfileId,ProfileNo)  OUTPUT INSERTED.DynamicFormDataId VALUES " +
                                        "(@LockedUserId,@IsLocked,@DynamicFormSectionGridAttributeId,@GridSortOrderByNo,@SortOrderByNo,@DynamicFormDataGridId,@DynamicFormItem,@DynamicFormId,@SessionId,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@IsSendApproval,@FileProfileSessionID,@ProfileId,@ProfileNo);\n\r";
                                result.DynamicFormData[i].NewDynamicFormDataId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                                i++;
                            }
                        }
                        catch (Exception exp)
                        {
                            await Delete(DynamicFormId);
                            throw new Exception(exp.Message, exp);
                        }

                    }
                }
                return result;
            }
            catch (Exception exp)
            {
                await Delete(DynamicFormId);
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<DynamicFormDataListItems> InsertDynamicFormDataSectionLock(DynamicFormDataListItems result, long DynamicFormId, string? IsGrid, long NewDynamicFormDataId)
        {
            try
            {
                if (result.DynamicFormDataSectionLock != null && result.DynamicFormDataSectionLock.Count() > 0)
                {
                    using (var connection = CreateConnection())
                    {
                        try
                        {
                            foreach (var dynamicFormDataSectionLock in result.DynamicFormDataSectionLock)
                            {
                                var newDynamicFormDataId = result.DynamicFormData.FirstOrDefault(f => f.DynamicFormDataId == dynamicFormDataSectionLock.DynamicFormDataId)?.NewDynamicFormDataId;
                                if (IsGrid == "IsGrid")
                                {
                                    newDynamicFormDataId = NewDynamicFormDataId;
                                }
                                if (newDynamicFormDataId > 0)
                                {
                                    var newDynamicFormSectionId = result.DynamicFormSection.FirstOrDefault(f => f.DynamicFormSectionId == dynamicFormDataSectionLock.DynamicFormSectionId)?.DynamicFormCloneSectionId;
                                    if (IsGrid == "IsGrid")
                                    {
                                        newDynamicFormSectionId = dynamicFormDataSectionLock.DynamicFormSectionId;
                                    }
                                    if (newDynamicFormSectionId > 0)
                                    {
                                        var parameters = new DynamicParameters();
                                        parameters.Add("DynamicFormDataId", newDynamicFormDataId);
                                        parameters.Add("DynamicFormSectionId", newDynamicFormSectionId);
                                        parameters.Add("IsLocked", dynamicFormDataSectionLock.IsLocked == true ? true : null);
                                        parameters.Add("LockedUserId", dynamicFormDataSectionLock.LockedUserId);
                                        var query = "INSERT INTO DynamicFormDataSectionLock(DynamicFormDataId,DynamicFormSectionId,IsLocked,LockedUserId) OUTPUT INSERTED.DynamicFormDataSectionLockId VALUES " +
                                 "(@DynamicFormDataId,@DynamicFormSectionId,@IsLocked,@LockedUserId)";
                                        var dynamicFormDataSectionLockId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                                    }
                                }
                            }
                        }
                        catch (Exception exp)
                        {
                            await Delete(DynamicFormId);
                            throw new Exception(exp.Message, exp);
                        }

                    }
                }
                return result;
            }
            catch (Exception exp)
            {
                await Delete(DynamicFormId);
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormDataListItems> InsertDynamicFormApproved(DynamicFormDataListItems result, long DynamicFormId, string? IsGrid, long NewDynamicFormDataId)
        {
            try
            {
                if (result.DynamicFormApproved != null && result.DynamicFormApproved.Count() > 0)
                {
                    using (var connection = CreateConnection())
                    {
                        try
                        {
                            var querys = string.Empty;
                            foreach (var dynamicFormApproved in result.DynamicFormApproved)
                            {
                                var newDynamicFormDataId = result.DynamicFormData.FirstOrDefault(f => f.DynamicFormDataId == dynamicFormApproved.DynamicFormDataId)?.NewDynamicFormDataId;
                                if (IsGrid == "IsGrid")
                                {
                                    newDynamicFormDataId = NewDynamicFormDataId;
                                }
                                if (newDynamicFormDataId > 0)
                                {
                                    var parameters = new DynamicParameters();
                                    parameters.Add("DynamicFormApprovalId", dynamicFormApproved.DynamicFormApprovalId);
                                    parameters.Add("DynamicFormDataId", newDynamicFormDataId);
                                    parameters.Add("UserId", dynamicFormApproved.UserId);
                                    var query = "INSERT INTO DynamicFormApproved(DynamicFormApprovalId,DynamicFormDataId,UserId)  " +
                                        "OUTPUT INSERTED.DynamicFormApprovedId VALUES " +
                                        "(@DynamicFormApprovalId,@DynamicFormDataId,@UserId)";
                                    var DynamicFormApprovedId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);

                                    if (result.DynamicFormApprovedChanged != null && result.DynamicFormApprovedChanged.Count() > 0)
                                    {
                                        foreach (var items in result.DynamicFormApprovedChanged)
                                        {
                                            if (items.UserId > 0)
                                            {
                                                querys += "INSERT INTO [DynamicFormApprovedChanged](DynamicFormApprovedId,UserId) OUTPUT INSERTED.DynamicFormApprovedChangedId " +
                                           "VALUES (" + DynamicFormApprovedId + "," + items.UserId + ");\n";
                                            }
                                        }
                                    }

                                }
                            }
                            if (!string.IsNullOrEmpty(querys))
                            {
                                await connection.QuerySingleOrDefaultAsync<long>(querys);
                            }
                        }
                        catch (Exception exp)
                        {
                            await Delete(DynamicFormId);
                            throw new Exception(exp.Message, exp);
                        }

                    }
                }
                return result;
            }
            catch (Exception exp)
            {
                await Delete(DynamicFormId);
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormDataListItems> InsertDynamicFormWorkFlowForm(DynamicFormDataListItems result, long NewDynamicFormId, string? IsGrid, long NewDynamicFormDataId)
        {
            try
            {
                if (result.DynamicFormWorkFlowForm != null && result.DynamicFormWorkFlowForm.Count() > 0)
                {
                    using (var connection = CreateConnection())
                    {
                        try
                        {
                            foreach (var value in result.DynamicFormWorkFlowForm)
                            {
                                var newDynamicFormDataId = result.DynamicFormData.FirstOrDefault(f => f.DynamicFormDataId == value.DynamicFormDataId)?.NewDynamicFormDataId;
                                if (IsGrid == "IsGrid")
                                {
                                    newDynamicFormDataId = NewDynamicFormDataId;
                                }
                                if (newDynamicFormDataId > 0)
                                {
                                    var query = string.Empty;
                                    var parameters = new DynamicParameters();
                                    parameters.Add("DynamicFormDataId", newDynamicFormDataId);
                                    parameters.Add("SequenceNo", value.SequenceNo);
                                    parameters.Add("UserId", value.UserId);
                                    parameters.Add("IsAllowDelegateUserForm", value.IsAllowDelegateUserForm == true ? true : null);
                                    query = "INSERT INTO [DynamicFormWorkFlowForm](DynamicFormDataId,UserId,SequenceNo,IsAllowDelegateUserForm) OUTPUT INSERTED.DynamicFormWorkFlowFormId " +
                                                       "VALUES (@DynamicFormDataId,@UserId,@SequenceNo,@IsAllowDelegateUserForm);\n";
                                    var dynamicFormWorkFlowFormId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                                    var querys = string.Empty;
                                    if (result.DynamicFormWorkFlowApprovedForm != null && result.DynamicFormWorkFlowApprovedForm.Count() > 0)
                                    {
                                        var oldItems = result.DynamicFormWorkFlowApprovedForm.Where(w => w.DynamicFormWorkFlowFormID == value.DynamicFormWorkFlowFormId).ToList();
                                        if (oldItems != null && oldItems.Count() > 0)
                                        {
                                            foreach (var items in oldItems)
                                            {
                                                var parameters1 = new DynamicParameters();
                                                parameters1.Add("DynamicFormWorkFlowFormID", dynamicFormWorkFlowFormId);
                                                parameters1.Add("SortBy", items.SortBy);
                                                parameters1.Add("UserId", items.UserID);
                                                var Newquery = "INSERT INTO [DynamicFormWorkFlowApprovedForm](DynamicFormWorkFlowFormID,UserId,SortBy) OUTPUT INSERTED.DynamicFormWorkFlowApprovedFormID " +
                                                      "VALUES (@DynamicFormWorkFlowFormID,@UserId,@SortBy);\n";
                                                var dynamicFormWorkFlowApprovedFormID = await connection.QuerySingleOrDefaultAsync<long>(Newquery, parameters1);
                                                if (result.DynamicFormWorkFlowApprovedFormChanged != null && result.DynamicFormWorkFlowApprovedFormChanged.Count() > 0)
                                                {
                                                    var oldIApptemsList = result.DynamicFormWorkFlowApprovedFormChanged.Where(w => w.DynamicFormWorkFlowApprovedFormID == items.DynamicFormWorkFlowApprovedFormID).ToList();
                                                    if (oldIApptemsList != null && oldIApptemsList.Count() > 0)
                                                    {
                                                        foreach (var itemsList in oldIApptemsList)
                                                        {
                                                            if (itemsList.UserId > 0)
                                                            {
                                                                querys += "INSERT INTO [DynamicFormWorkFlowApprovedFormChanged](DynamicFormWorkFlowApprovedFormID,UserId) OUTPUT INSERTED.DynamicFormWorkFlowApprovedFormChangedID " +
                                                           "VALUES (" + dynamicFormWorkFlowApprovedFormID + "," + itemsList.UserId + ");\n";
                                                            }
                                                        }
                                                    }

                                                }
                                            }
                                        }

                                    }

                                    if (result.DynamicFormWorkFlowSectionForm != null && result.DynamicFormWorkFlowSectionForm.Count() > 0)
                                    {
                                        var oldItems = result.DynamicFormWorkFlowSectionForm.Where(w => w.DynamicFormWorkFlowFormID == value.DynamicFormWorkFlowFormId).ToList();
                                        if (oldItems != null && oldItems.Count() > 0)
                                        {
                                            foreach (var items in oldItems)
                                            {
                                                if (items.DynamicFormSectionID > 0)
                                                {
                                                    var newDynamicFormSectionId = result.DynamicFormSection.FirstOrDefault(f => f.DynamicFormSectionId == items.DynamicFormSectionID)?.DynamicFormCloneSectionId;
                                                    if (IsGrid == "IsGrid")
                                                    {
                                                        newDynamicFormSectionId = items.DynamicFormSectionID;
                                                    }
                                                    if (newDynamicFormSectionId > 0)
                                                    {
                                                        querys += "INSERT INTO [DynamicFormWorkFlowSectionForm](DynamicFormWorkFlowFormId,DynamicFormSectionId) OUTPUT INSERTED.DynamicFormWorkFlowSectionFormId " +
                                                   "VALUES (" + dynamicFormWorkFlowFormId + "," + newDynamicFormSectionId + ");\n";
                                                    }
                                                }
                                            }
                                        }

                                    }
                                    if (result.DynamicFormWorkFlowFormDelegate != null && result.DynamicFormWorkFlowFormDelegate.Count() > 0)
                                    {
                                        var oldIApptems = result.DynamicFormWorkFlowFormDelegate.Where(w => w.DynamicFormWorkFlowFormID == value.DynamicFormWorkFlowFormId).ToList();
                                        if (oldIApptems != null && oldIApptems.Count() > 0)
                                        {
                                            foreach (var items in oldIApptems)
                                            {
                                                if (items.UserID > 0)
                                                {
                                                    querys += "INSERT INTO [DynamicFormWorkFlowFormDelegate](DynamicFormWorkFlowFormID,UserId) OUTPUT INSERTED.DynamicFormWorkFlowFormDelegateID " +
                                               "VALUES (" + dynamicFormWorkFlowFormId + "," + items.UserID + ");\n";
                                                }
                                            }
                                        }

                                    }
                                    if (!string.IsNullOrEmpty(querys))
                                    {
                                        await connection.QuerySingleOrDefaultAsync<long>(querys);
                                    }
                                }
                            }
                        }
                        catch (Exception exp)
                        {
                            await Delete(NewDynamicFormId);
                            throw new Exception(exp.Message, exp);
                        }
                    }
                }
                return result;
            }
            catch (Exception exp)
            {
                await Delete(NewDynamicFormId);
                throw new Exception(exp.Message, exp);
            }
        }
        public class DynamicFormDataSortOrderByNo
        {
            public long? SortOrderByNo { get; set; } = 1;
            public long? GridSortOrderByNo { get; set; } = 1;
        }
        public async Task<DynamicFormDataSortOrderByNo> GeDynamicFormDataSortOrdrByNo(long? DynamicFormId, long NewDynamicFormId, long? NewDynamicFormDataGridId)
        {
            try
            {
                DynamicFormDataSortOrderByNo DynamicFormDataSortOrderByNo = new DynamicFormDataSortOrderByNo();
                long? SortOrderBy = 1; long? GridSortOrderByNo = 1;
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormId", DynamicFormId);
                parameters.Add("DynamicFormDataGridId", NewDynamicFormDataGridId);
                query += "SELECT DynamicFormId,SortOrderByNo FROM DynamicFormData Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormId = @DynamicFormId order by  SortOrderByNo desc;";
                query += "SELECT DynamicFormId,GridSortOrderByNo FROM DynamicFormData Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormDataGridId=@DynamicFormDataGridId AND DynamicFormId = @DynamicFormId   order by  GridSortOrderByNo desc";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    var result1 = results.Read<DynamicFormData>().FirstOrDefault();
                    var result2 = results.Read<DynamicFormData>().FirstOrDefault();
                    if (result1 != null && result1.SortOrderByNo > 0)
                    {
                        SortOrderBy = result1.SortOrderByNo + 1;
                    }
                    else
                    {
                        SortOrderBy = 1;
                    }
                    DynamicFormDataSortOrderByNo.SortOrderByNo = SortOrderBy;
                    if (result2 != null && result2.GridSortOrderByNo > 0)
                    {
                        GridSortOrderByNo = result2.GridSortOrderByNo + 1;
                    }
                    else
                    {
                        GridSortOrderByNo = 1;
                    }
                    DynamicFormDataSortOrderByNo.GridSortOrderByNo = GridSortOrderByNo;
                }
                return DynamicFormDataSortOrderByNo;
            }
            catch (Exception exp)
            {
                await Delete(NewDynamicFormId);
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<DynamicFormDataListItems> InsertDynamicFormDataGrid(DynamicFormDataListItems result, long? UserId, DateTime DateTime, long NewDynamicFormId)
        {
            try
            {
                if (result.DynamicFormDataGrid != null && result.DynamicFormDataGrid.Count() > 0)
                {
                    using (var connection = CreateConnection())
                    {
                        try
                        {
                            int i = 0;
                            foreach (var dynamicFormData in result.DynamicFormDataGrid)
                            {

                                long? NewDynamicFormSectionGridAttributeId = null;
                                if (dynamicFormData.DynamicFormSectionGridAttributeId > 0)
                                {
                                    NewDynamicFormSectionGridAttributeId = result.DynamicFormSectionAttribute.FirstOrDefault(f => f.DynamicFormSectionAttributeId == dynamicFormData.DynamicFormSectionGridAttributeId)?.NewDynamicFormSectionAttributeId;
                                }
                                long? NewDynamicFormDataGridId = null;
                                if (dynamicFormData.DynamicFormDataGridId > 0)
                                {
                                    NewDynamicFormDataGridId = result.DynamicFormData.FirstOrDefault(f => f.DynamicFormDataId == dynamicFormData.DynamicFormDataGridId)?.NewDynamicFormDataId;
                                }
                                if (NewDynamicFormDataGridId > 0)
                                {
                                    var res = await GeDynamicFormDataSortOrdrByNo(dynamicFormData.DynamicFormId, NewDynamicFormId, NewDynamicFormDataGridId);
                                    var parameters = new DynamicParameters();
                                    parameters.Add("DynamicFormItem", dynamicFormData.DynamicFormItem, DbType.String);
                                    parameters.Add("DynamicFormId", dynamicFormData.DynamicFormId);
                                    parameters.Add("SessionId", Guid.NewGuid(), DbType.Guid);
                                    parameters.Add("AddedByUserID", UserId);
                                    parameters.Add("ModifiedByUserID", UserId);
                                    parameters.Add("AddedDate", DateTime, DbType.DateTime);
                                    parameters.Add("ModifiedDate", DateTime, DbType.DateTime);
                                    parameters.Add("StatusCodeID", dynamicFormData.StatusCodeID);
                                    parameters.Add("IsSendApproval", dynamicFormData.IsSendApproval);
                                    parameters.Add("FileProfileSessionID", dynamicFormData.FileProfileSessionID, DbType.Guid);
                                    parameters.Add("DynamicFormDataGridId", NewDynamicFormDataGridId);
                                    parameters.Add("DynamicFormSectionGridAttributeId", NewDynamicFormSectionGridAttributeId);
                                    parameters.Add("ProfileId", dynamicFormData.ProfileId);
                                    parameters.Add("ProfileNo", dynamicFormData.ProfileNo, DbType.String);
                                    parameters.Add("SortOrderByNo", res.SortOrderByNo);
                                    parameters.Add("GridSortOrderByNo", res.GridSortOrderByNo);
                                    parameters.Add("IsLocked", dynamicFormData.IsLocked);
                                    parameters.Add("LockedUserId", dynamicFormData.LockedUserId);
                                    var query = "INSERT INTO DynamicFormData(LockedUserId,IsLocked,DynamicFormSectionGridAttributeId,GridSortOrderByNo,SortOrderByNo,DynamicFormDataGridId,DynamicFormItem,DynamicFormId,SessionId,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,IsSendApproval,FileProfileSessionID,ProfileId,ProfileNo)  OUTPUT INSERTED.DynamicFormDataId VALUES " +
                                            "(@LockedUserId,@IsLocked,@DynamicFormSectionGridAttributeId,@GridSortOrderByNo,@SortOrderByNo,@DynamicFormDataGridId,@DynamicFormItem,@DynamicFormId,@SessionId,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@IsSendApproval,@FileProfileSessionID,@ProfileId,@ProfileNo);\n\r";
                                    var NewDynamicFormDataId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                                    result.DynamicFormDataGrid[i].NewDynamicFormDataId = NewDynamicFormDataId;
                                    var result1 = await GetDynamicFormDataGridOne(dynamicFormData.DynamicFormDataGridId.ToString(), dynamicFormData.DynamicFormDataId.ToString());
                                    await InsertDynamicFormDataSectionLock(result1, (long)dynamicFormData.DynamicFormId, "IsGrid", NewDynamicFormDataId);
                                    await InsertDynamicFormApproved(result1, (long)dynamicFormData.DynamicFormId, "IsGrid", NewDynamicFormDataId);
                                    await InsertDynamicFormWorkFlowForm(result1, (long)dynamicFormData.DynamicFormId, "IsGrid", NewDynamicFormDataId);
                                    if (result1.DynamicFormDataGrid != null && result1.DynamicFormDataGrid.Count() > 0)
                                    {
                                        foreach (var dynamicFormDataGrid in result1.DynamicFormData)
                                        {
                                            await InsertDynamicFormDataGriNestedd(result1, UserId, DateTime, (long)dynamicFormDataGrid.DynamicFormId, NewDynamicFormDataId);
                                        }
                                    }

                                }
                                i++;
                            }
                        }
                        catch (Exception exp)
                        {
                            await Delete(NewDynamicFormId);
                            throw new Exception(exp.Message, exp);
                        }

                    }
                }
                return result;
            }
            catch (Exception exp)
            {
                await Delete(NewDynamicFormId);
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormDataListItems> GetDynamicFormDataGridOne(string? DynamicFormDataIds, string? DynamicFormDataId)
        {
            try
            {
                DynamicFormDataListItems result = new DynamicFormDataListItems();
                if (!string.IsNullOrEmpty(DynamicFormDataIds))
                {
                    var query1 = string.Empty;
                    query1 += "select t1.* from DynamicFormData t1 where (t1.IsDeleted is null OR t1.IsDeleted=0) AND t1.DynamicFormDataID IN(" + DynamicFormDataIds + ");\n";
                    query1 += "select t1.* from DynamicFormData t1 where (t1.IsDeleted is null OR t1.IsDeleted=0) AND t1.DynamicFormDataGridID IN(" + DynamicFormDataId + ");\n";
                    query1 += "select t1.* from DynamicFormApproved t1 where t1.DynamicFormDataID IN(" + DynamicFormDataIds + ");\n";
                    query1 += "select t1.* from DynamicFormApprovedChanged t1 Where t1.DynamicFormApprovedID IN(select t2.DynamicFormApprovedID from DynamicFormApproved t2 where t2.DynamicFormDataID IN(" + DynamicFormDataIds + "));\n";
                    query1 += "select t1.* from DynamicFormDataSectionLock t1 where t1.DynamicFormDataID IN(" + DynamicFormDataIds + ");\n";
                    query1 += "select t1.* from DynamicFormWorkFlowForm t1 where t1.DynamicFormDataID IN(" + DynamicFormDataIds + ");\n";
                    query1 += "select t1.* from DynamicFormWorkFlowSectionForm t1 Where t1.DynamicFormWorkFlowFormID IN(select t2.DynamicFormWorkFlowFormID from DynamicFormWorkFlowForm t2 Where t2.DynamicFormDataID IN(" + DynamicFormDataIds + "));\n";
                    query1 += "select t1.* from DynamicFormWorkFlowFormDelegate t1 Where t1.DynamicFormWorkFlowFormID IN(select t2.DynamicFormWorkFlowFormID from DynamicFormWorkFlowForm t2 Where t2.DynamicFormDataID IN(" + DynamicFormDataIds + "));\n";
                    query1 += "select t1.* from DynamicFormWorkFlowApprovedForm t1 Where t1.DynamicFormWorkFlowFormID IN(select t2.DynamicFormWorkFlowFormID from DynamicFormWorkFlowForm t2 Where t2.DynamicFormDataID IN(" + DynamicFormDataIds + "));\n";
                    query1 += "select t1.* from DynamicFormWorkFlowApprovedFormChanged t1 Where t1.DynamicFormWorkFlowApprovedFormID IN(select t2.DynamicFormWorkFlowApprovedFormID from DynamicFormWorkFlowApprovedForm t2 Where t2.DynamicFormWorkFlowFormID IN(select t3.DynamicFormWorkFlowFormID from DynamicFormWorkFlowForm t3 Where t3.DynamicFormDataID IN(" + DynamicFormDataIds + ")));\n";
                    using (var connection = CreateConnection())
                    {
                        var results = await connection.QueryMultipleAsync(query1);
                        result.DynamicFormData = results.ReadAsync<DynamicFormData>().Result.ToList();
                        result.DynamicFormDataGrid = results.ReadAsync<DynamicFormData>().Result.ToList();
                        result.DynamicFormApproved = results.ReadAsync<DynamicFormApproved>().Result.ToList();
                        result.DynamicFormApprovedChanged = results.ReadAsync<DynamicFormApprovedChanged>().Result.ToList();
                        result.DynamicFormDataSectionLock = results.ReadAsync<DynamicFormDataSectionLock>().Result.ToList();
                        result.DynamicFormWorkFlowForm = results.ReadAsync<DynamicFormWorkFlowForm>().Result.ToList();
                        result.DynamicFormWorkFlowSectionForm = results.ReadAsync<DynamicFormWorkFlowSectionForm>().Result.ToList();
                        result.DynamicFormWorkFlowFormDelegate = results.ReadAsync<DynamicFormWorkFlowFormDelegate>().Result.ToList();
                        result.DynamicFormWorkFlowApprovedForm = results.ReadAsync<DynamicFormWorkFlowApprovedForm>().Result.ToList();
                        result.DynamicFormWorkFlowApprovedFormChanged = results.ReadAsync<DynamicFormWorkFlowApprovedFormChanged>().Result.ToList();
                    }
                }
                return result;
            }
            catch (Exception exp)
            {
                // await Delete(DynamicFormId);
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormDataListItems> InsertDynamicFormDataGriNestedd(DynamicFormDataListItems result, long? UserId, DateTime DateTime, long NewDynamicFormId, long? NewDynamicDataFormId)
        {
            try
            {
                if (result.DynamicFormDataGrid != null && result.DynamicFormDataGrid.Count() > 0)
                {
                    using (var connection = CreateConnection())
                    {
                        try
                        {
                            int i = 0;
                            foreach (var dynamicFormData in result.DynamicFormDataGrid)
                            {
                                if (NewDynamicDataFormId > 0)
                                {
                                    var res = await GeDynamicFormDataSortOrdrByNo(dynamicFormData.DynamicFormId, NewDynamicFormId, NewDynamicDataFormId);
                                    var parameters = new DynamicParameters();
                                    parameters.Add("DynamicFormItem", dynamicFormData.DynamicFormItem, DbType.String);
                                    parameters.Add("DynamicFormId", dynamicFormData.DynamicFormId);
                                    parameters.Add("SessionId", Guid.NewGuid(), DbType.Guid);
                                    parameters.Add("AddedByUserID", UserId);
                                    parameters.Add("ModifiedByUserID", UserId);
                                    parameters.Add("AddedDate", DateTime, DbType.DateTime);
                                    parameters.Add("ModifiedDate", DateTime, DbType.DateTime);
                                    parameters.Add("StatusCodeID", dynamicFormData.StatusCodeID);
                                    parameters.Add("IsSendApproval", dynamicFormData.IsSendApproval);
                                    parameters.Add("FileProfileSessionID", dynamicFormData.FileProfileSessionID, DbType.Guid);
                                    parameters.Add("DynamicFormDataGridId", NewDynamicDataFormId);
                                    parameters.Add("DynamicFormSectionGridAttributeId", dynamicFormData.DynamicFormSectionGridAttributeId);
                                    parameters.Add("ProfileId", dynamicFormData.ProfileId);
                                    parameters.Add("ProfileNo", dynamicFormData.ProfileNo, DbType.String);
                                    parameters.Add("SortOrderByNo", res.SortOrderByNo);
                                    parameters.Add("GridSortOrderByNo", res.GridSortOrderByNo);
                                    parameters.Add("IsLocked", dynamicFormData.IsLocked);
                                    parameters.Add("LockedUserId", dynamicFormData.LockedUserId);
                                    var query = "INSERT INTO DynamicFormData(LockedUserId,IsLocked,DynamicFormSectionGridAttributeId,GridSortOrderByNo,SortOrderByNo,DynamicFormDataGridId,DynamicFormItem,DynamicFormId,SessionId,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,IsSendApproval,FileProfileSessionID,ProfileId,ProfileNo)  OUTPUT INSERTED.DynamicFormDataId VALUES " +
                                            "(@LockedUserId,@IsLocked,@DynamicFormSectionGridAttributeId,@GridSortOrderByNo,@SortOrderByNo,@DynamicFormDataGridId,@DynamicFormItem,@DynamicFormId,@SessionId,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@IsSendApproval,@FileProfileSessionID,@ProfileId,@ProfileNo);\n\r";
                                    var NewDynamicFormDataId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                                    result.DynamicFormDataGrid[i].NewDynamicFormDataId = NewDynamicFormDataId;
                                    var result1 = await GetDynamicFormDataGridOne(dynamicFormData.DynamicFormDataGridId.ToString(), dynamicFormData.DynamicFormDataId.ToString());
                                    await InsertDynamicFormDataSectionLock(result1, (long)dynamicFormData.DynamicFormId, "IsGrid", NewDynamicFormDataId);
                                    await InsertDynamicFormApproved(result1, (long)dynamicFormData.DynamicFormId, "IsGrid", NewDynamicFormDataId);
                                    await InsertDynamicFormWorkFlowForm(result1, (long)dynamicFormData.DynamicFormId, "IsGrid", NewDynamicFormDataId);
                                    if (result1.DynamicFormDataGrid != null && result1.DynamicFormDataGrid.Count() > 0)
                                    {
                                        foreach (var dynamicFormDataGrid in result1.DynamicFormDataGrid)
                                        {
                                            await InsertDynamicFormDataGriNestedd(result1, UserId, DateTime, (long)dynamicFormDataGrid.DynamicFormId, NewDynamicFormDataId);
                                        }
                                    }

                                }
                                i++;
                            }
                        }
                        catch (Exception exp)
                        {
                            await Delete(NewDynamicFormId);
                            throw new Exception(exp.Message, exp);
                        }

                    }
                }
                return result;
            }
            catch (Exception exp)
            {
                await Delete(NewDynamicFormId);
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task Delete(long id)
        {
            if (id > 0)
            {
                try
                {
                    using (var connection = CreateConnection())
                    {
                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("DynamicFormID", id);
                            var DynamicFormDataIds = string.Empty;
                            var query1 = "SELECT DynamicFormDataId FROM DynamicFormData Where DynamicFormId = @DynamicFormId;";
                            var resultData = (await connection.QueryAsync<DynamicFormData>(query1, parameters)).ToList();

                            if (resultData != null && resultData.Count() > 0)
                            {
                                DynamicFormDataIds = string.Join(',', resultData.Select(s => s.DynamicFormDataId).ToList());
                            }
                            var query = string.Empty;
                            if (!string.IsNullOrEmpty(DynamicFormDataIds))
                            {
                                query += "Delete from DynamicFormWorkFlowApprovedFormChanged  Where DynamicFormWorkFlowApprovedFormID IN(select t2.DynamicFormWorkFlowApprovedFormID from DynamicFormWorkFlowApprovedForm t2 Where t2.DynamicFormWorkFlowFormID IN(select t3.DynamicFormWorkFlowFormID from DynamicFormWorkFlowForm t3 Where t3.DynamicFormDataID IN(" + DynamicFormDataIds + ")));\n";
                                query += "Delete from DynamicFormWorkFlowApprovedForm  Where DynamicFormWorkFlowFormID IN(select t2.DynamicFormWorkFlowFormID from DynamicFormWorkFlowForm t2 Where t2.DynamicFormDataID IN(" + DynamicFormDataIds + "));\n";
                                query += "Delete from DynamicFormWorkFlowFormDelegate  Where DynamicFormWorkFlowFormID IN(select t2.DynamicFormWorkFlowFormID from DynamicFormWorkFlowForm t2 Where t2.DynamicFormDataID IN(" + DynamicFormDataIds + "));\n";
                                query += "Delete from DynamicFormWorkFlowSectionForm Where DynamicFormWorkFlowFormID IN(select t2.DynamicFormWorkFlowFormID from DynamicFormWorkFlowForm t2 Where t2.DynamicFormDataID IN(" + DynamicFormDataIds + "));\n";
                                query += "Delete from DynamicFormWorkFlowForm  where DynamicFormDataID IN(" + DynamicFormDataIds + ");\n";
                                query += "Delete from DynamicFormDataSectionLock  where DynamicFormDataID IN(" + DynamicFormDataIds + ");\n";
                                query += "Delete from DynamicFormApprovedChanged  Where DynamicFormApprovedID IN(select t2.DynamicFormApprovedID from DynamicFormApproved t2 where t2.DynamicFormDataID IN(" + DynamicFormDataIds + "));\n";
                                query += "Delete from DynamicFormApproved  where DynamicFormDataID IN(" + DynamicFormDataIds + ");\n";
                                query += "Delete from DynamicFormData  where  DynamicFormDataGridID IN(" + DynamicFormDataIds + ");\n";
                                query += "Delete FROM DynamicFormData Where  DynamicFormId = @DynamicFormId;";
                                await connection.ExecuteAsync(query, parameters);
                            }
                            var query2 = string.Empty;
                            query2 += "Delete from DynamicFormSectionAttributeSecurity  where DynamicFormSectionAttributeID IN(select t2.DynamicFormSectionAttributeID from DynamicFormSectionAttribute t2 Where t2.DynamicFormSectionID IN (select t3.DynamicFormSectionID from DynamicFormSection t3 Where t3.DynamicFormID=@DynamicFormID));\n";
                            query2 += "Delete from DynamicFormSectionSecurity  where DynamicFormSectionID IN(select t2.DynamicFormSectionID from DynamicFormSection t2 Where t2.DynamicFormID=@DynamicFormID);\n";
                            query2 += "Delete from DynamicFormSectionAttributeSection  where DynamicFormSectionAttributeSectionParentID IN \r\n(select t2.DynamicFormSectionAttributeSectionParentID from DynamicFormSectionAttributeSectionParent t2 where t2.DynamicFormSectionAttributeID IN\r\n(select t3.DynamicFormSectionAttributeID from DynamicFormSectionAttribute t3 Where t3.DynamicFormSectionID IN(Select t4.DynamicFormSectionID from DynamicFormSection t4 where t4.DynamicFormID= @DynamicFormID)));\n";
                            query2 += "Delete from DynamicFormSectionAttributeSectionParent  where DynamicFormSectionAttributeID IN(select t2.DynamicFormSectionAttributeID from DynamicFormSectionAttribute t2 Where t2.DynamicFormSectionID IN (select t3.DynamicFormSectionID from DynamicFormSection t3 Where t3.DynamicFormID=@DynamicFormID));\n";
                            query2 += "Delete from DynamicFormSectionAttribute  Where DynamicFormSectionID IN (select t2.DynamicFormSectionID from DynamicFormSection t2 Where t2.DynamicFormID=@DynamicFormID);\n";
                            query2 += "Delete from DynamicFormWorkFlowApproval  where DynamicFormWorkFlowID IN(select t2.DynamicFormWorkFlowID from DynamicFormWorkFlow t2 Where t2.DynamicFormID=@DynamicFormID);\n";
                            query2 += "Delete from DynamicFormWorkFlowSection  Where DynamicFormWorkFlowID IN (select t2.DynamicFormWorkFlowID from DynamicFormWorkFlow t2 where t2.DynamicFormID=@DynamicFormID);\n";
                            query2 += "Delete from DynamicFormWorkFlow  Where DynamicFormID=@DynamicFormID;\n";
                            query2 += "Delete from DynamicFormApproval  Where DynamicFormID=@DynamicFormID;\n";
                            query2 += "Delete from DynamicFormSection  Where DynamicFormID=@DynamicFormID;\n";
                            query2 += "Delete from DynamicForm  where  ID=@DynamicFormID;\n";

                            var rowsAffected = await connection.ExecuteAsync(query2, parameters);
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
}
