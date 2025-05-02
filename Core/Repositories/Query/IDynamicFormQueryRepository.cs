using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Rpc.Context.AttributeContext.Types;

namespace Core.Repositories.Query
{
    public interface IDynamicFormQueryRepository : IQueryRepository<DynamicForm>
    {
        Task<DynamicForm> GetAllSelectedList(Guid? sessionId, long? DynamicFormDataId);
        Task<IReadOnlyList<DynamicForm>> GetAllAsync(long? userId);
        Task<IReadOnlyList<DynamicForm>> GetAllByNoGridFormAsync(long? userId);
        Task<IReadOnlyList<DynamicForm>> GetAllByGridFormAsync(long? userId);
        Task<DynamicForm> GetDynamicFormByIdAsync(long? Id);
        Task<DynamicForm> GetDynamicFormBySessionIdAsync(Guid? SessionId);
        Task<long> Insert(DynamicForm dynamicForm);
        Task<long> Update(DynamicForm dynamicForm);
        Task<DynamicForm> Delete(DynamicForm dynamicForm);
        Task<DynamicFormData> GetDynamicFormDataBySessionOneAsync(Guid? SessionId);
        DynamicForm GetDynamicFormScreenNameCheckValidation(string? value, long id);
        Task<DynamicForm> GetDynamicFormScreenNameDataCheckValidation(string? value);
        Task<DynamicFormSection> InsertOrUpdateDynamicFormSection(DynamicFormSection dynamicFormSection);
        Task<IReadOnlyList<DynamicFormSection>> GetDynamicFormSectionAsync(long? dynamicFormId);
        Task<DynamicFormSectionAttribute> InsertOrUpdateDynamicFormSectionAttribute(DynamicFormSectionAttribute dynamicFormSectionAttribute);
        Task<IReadOnlyList<DynamicFormSectionAttribute>> GetDynamicFormSectionAttributeAllAsync(long? DynamicFormId);
        Task<IReadOnlyList<DynamicFormSectionAttribute>> GetDynamicFormSectionAttributeAsync(long? dynamicFormSectionId);
        Task<long> DeleteDynamicFormSection(DynamicFormSection dynamicFormSection);
        Task<DynamicFormSection> UpdateDynamicFormSectionSortOrder(DynamicFormSection dynamicFormSection);
        Task<long> DeleteDynamicFormSectionAttribute(DynamicFormSectionAttribute dynamicFormSectionAttribute);
        Task<DynamicFormSectionAttribute> UpdateDynamicFormSectionAttributeSortOrder(DynamicFormSectionAttribute dynamicFormSectionAttribute);
        Task<long> InsertDynamicFormAttributeSection(long dynamicFormSectionId, IEnumerable<AttributeHeader> attributeIds, long? userId);
        Task<DynamicFormData> InsertOrUpdateDynamicFormData(DynamicFormData dynamicFormData);
        Task<DynamicFormData> GetDynamicFormDataBySessionIdAsync(Guid? SessionId);
        Task<DocumentsModel> GetDynamicFormDataBySessionIdForDMSAsync(Guid? SessionId);
        Task<IReadOnlyList<DynamicFormData>> GetDynamicFormDataByIdAsync(long? id, long? userId, long? DynamicFormDataGridId, long? DynamicFormSectionGridAttributeId, Guid? DynamicFormDataSessionId);
        Task<DynamicFormData> DeleteDynamicFormData(DynamicFormData dynamicFormData);
        Task<IReadOnlyList<DynamicFormApproval>> GetDynamicFormApprovalAsync(long? dynamicFormId);
        Task<DynamicFormApproval> InsertOrUpdateDynamicFormApproval(DynamicFormApproval dynamicFormApproval);
        Task<DynamicFormApproved> InsertOrUpdateDynamicFormDataApproved(DynamicFormApproved dynamicFormApproved);
        Task<DynamicFormApproved> DeleteDynamicFormDataApproved(DynamicFormApproved dynamicFormApproved);
        Task<DynamicFormApproval> DeleteDynamicFormApproval(DynamicFormApproval dynamicFormApproval);
        DynamicFormApproval GetDynamicFormApprovalUserCheckValidation(long? dynamicFormId, long? dynamicFormApprovalId, long? approvalUserId);
        Task<DynamicFormApproval> UpdateDynamicFormApprovalSortOrder(DynamicFormApproval dynamicFormApproval);
        Task<DynamicFormApproval> UpdateDescriptionDynamicFormApprovalField(DynamicFormApproval dynamicFormApproval);
        Task<DynamicFormSectionSecurity> InsertDynamicFormSectionSecurity(DynamicFormSectionSecurity value);
        Task<IReadOnlyList<DynamicFormSectionSecurity>> GetDynamicFormSectionSecurityList(long? Id);
        Task<long> DeleteDynamicFormSectionSecurity(long? Id, List<long?> Ids);
        Task<DynamicFormApproved> InsertDynamicFormApproved(DynamicFormApproved dynamicForm);
        Task<DynamicFormApproved> GetDynamicFormApprovedByID(long? dynamicFormDataId, long? approvalUserId);
        Task<DynamicFormData> InsertOrUpdateDynamicFormApproved(DynamicFormData dynamicFormData);
        Task<IReadOnlyList<DynamicFormApproved>> GetDynamicFormApprovedList(long? DynamicFormDataId);
        Task<DynamicFormApproved> UpdateDynamicFormApprovedByStaus(DynamicFormApproved dynamicFormApproved);
        Task<IReadOnlyList<DynamicFormSection>> GetDynamicFormSectionByIdAsync(long? id, long? UserId, long? dynamicFormDataId);
        Task<DynamicFormDataUpload> InsertDynamicFormDataUpload(DynamicFormDataUpload dynamicFormSection);
        Task<IReadOnlyList<DynamicFormSectionWorkFlow>> GetDynamicFormSectionWorkFlowByID(long? DynamicFormSectionId, long? UserId);
        Task<DynamicFormWorkFlow> InsertDynamicFormWorkFlow(DynamicFormWorkFlow value);
        Task<IReadOnlyList<DynamicFormWorkFlow>> GetDynamicFormWorkFlowAsync(long? dynamicFormId);

        Task<DynamicFormWorkFlow> DeleteDynamicFormWorkFlow(DynamicFormWorkFlow value);
        Task<IReadOnlyList<DynamicFormWorkFlowSection>> GetDynamicFormWorkFlowExits(long? dynamicFormId, long? userId, long? dynamicFormDataId);
        Task<DynamicFormWorkFlowSection> InsertOrUpdateFormWorkFlowSectionNoWorkFlow(List<DynamicFormWorkFlowSection> dynamicFormWorkFlowSections, long? dynamicFormDataId, long? userId);
        Task<IReadOnlyList<DynamicFormWorkFlowForm>> GetDynamicFormWorkFlowFormList(long? dynamicFormDataId, long? dynamicFormId);
        Task<DynamicFormWorkFlowForm> GetDynamicFormWorkFlowFormExits(long? dynamicFormWorkFlowSectionId, long? userId, long? dynamicFormDataId);

        Task<IReadOnlyList<DynamicFormDataWrokFlow>> GetDynamicFormWorkFlowListByUser(long? userId, long? dynamicFormDataId);
        Task<DynamicFormData> InsertCreateEmailFormData(DynamicFormData dynamicFormData);
        Task<ViewEmployee> GetEmployeeByUserIdIdAsync(long? userId);
        Task<DynamicFormSectionAttributeSecurity> InsertDynamicFormSectionAttributeSecurity(DynamicFormSectionAttributeSecurity value);
        Task<IReadOnlyList<DynamicFormSectionAttributeSecurity>> GetDynamicFormSectionAttributeSecurityList(long? Id);
        Task<long> DeleteDynamicFormSectionAttributeSecurity(long? Id, List<long?> Ids);

        Task<IReadOnlyList<DynamicFormData?>> GetDynamicFormDataApprovalList(long? userId);

        Task<DynamicFormData> UpdateDynamicFormDataSortOrder(DynamicFormData dynamicFormData);
        Task<DynamicFormDataUpload> InsertDmsDocumentDynamicFormData(DynamicFormDataUpload dynamicFormDataUpload);

        Task<DynamicFormReport> InsertDynamicFormReport(DynamicFormReport reportDocuments);
        Task<IReadOnlyList<DynamicFormReport>> GetDynamicFormReportList(long? DynamicFormId);
        Task<DynamicFormReport> DeleteDynamicFormReport(DynamicFormReport dynamicFormReport);
        Task<DynamicFormReport> GetDynamicFormReportOneData(Guid? SessionId);
        DynamicFormSectionAttribute GetDynamicFormSectionAttributeCheckValidation(long? dynamicFormId, long? dynamicFormSectionAttributeId, long? attributeId);
        Task<IReadOnlyList<ApplicationMasterParent>> GetDynamicFormApplicationMasterParentAsync(long? dynamicFormId);
        Task<IReadOnlyList<ApplicationMaster>> GetDynamicFormApplicationMasterAsync(long? dynamicFormId);
        Task<IReadOnlyList<DynamicFormSectionAttributeSectionParent>> GetDynamicFormSectionAttributeSectionParentAsync(long? dynamicFormSectionAttributeId);
        Task<DynamicFormSectionAttributeSectionParent> InsertOrUpdateDynamicFormSectionAttributeSectionParent(DynamicFormSectionAttributeSectionParent dynamicFormSection);
        Task<DynamicFormSectionAttributeSectionParent> DeleteDynamicFormSectionAttributeParent(DynamicFormSectionAttributeSectionParent value);
        Task<DynamicFormData> UpdateDynamicFormLocked(DynamicFormData dynamicFormData);
        Task<DynamicFormApprovedChanged> InsertDynamicFormApprovedChanged(DynamicFormApprovedChanged dynamicFormApprovedChanged);

        Task<DynamicFormWorkFlowApproval> InsertDynamicFormWorkFlowApproval(DynamicFormWorkFlowApproval dynamicFormWorkFlowApproval);
        Task<IReadOnlyList<DynamicFormWorkFlowApproval>> GetDynamicFormWorkFlowApprovalList(long? DynamicFormWorkFlowId, long? dynamicFormDataId);
        Task<DynamicFormWorkFlowApproval> DeleteDynamicFormWorkFlowApproval(DynamicFormWorkFlowApproval dynamicFormWorkFlowApproval);
        Task<DynamicFormWorkFlowApproval> UpdateDynamicFormWorkFlowApprovalSortOrder(DynamicFormWorkFlowApproval dynamicFormWorkFlowApproval);
        Task<IReadOnlyList<DynamicFormWorkFlowApprovedForm>> GetDynamicFormWorkFlowApprovedFormList(long? dynamicFormDataId);
        Task<DynamicFormWorkFlowApprovedForm> UpdateDynamicFormWorkFlowApprovedFormByUser(DynamicFormWorkFlowApprovedForm dynamicFormWorkFlowApprovedForm);
        Task<DynamicFormWorkFlowApprovedFormChanged> InsertDynamicFormWorkFlowApprovedFormChanged(DynamicFormWorkFlowApprovedFormChanged dynamicFormApprovedChanged);

        Task<DynamicFormWorkFlowApproval> InsertDynamicFormDataWorkFlowApproval(DynamicFormWorkFlowApproval dynamicFormWorkFlowApproval);
        Task<DynamicFormWorkFlowApproval> DeleteDynamicFormDataWorkFlowApproval(DynamicFormWorkFlowApproval dynamicFormWorkFlowApproval);
        DynamicFormWorkFlow GetDynamicFormWorkFlowSequenceNoExitsCheckValidation(long? dynamicFormId, long? dynamicFormWorkFlowId, int? SequenceNo);

        Task<IReadOnlyList<DynamicFormWorkFlowApprovedForm>> GetDynamicFormWorkFlowApprovedFormByList(long? userId, int? FlowStatusID);
        Task<IReadOnlyList<DynamicFormWorkFlowFormDelegate>> GetDynamicFormWorkFlowFormDelegateList(long? DynamicFormWorkFlowFormID);
        Task<DynamicFormWorkFlowFormDelegate> InsertDynamicFormWorkFlowFormDelegate(DynamicFormWorkFlowFormDelegate dynamicFormWorkFlowFormDelegate);
        DynamicFormWorkFlowForm GetDynamicFormDataWorkFlowSequenceNoExitsCheckValidation(long? dynamicFormId, long? dynamicFormWorkFlowId, int? SequenceNo);
        Task<DynamicFormWorkFlowForm> InsertDynamicFormWorkFlowForm(DynamicFormWorkFlowForm value);
        Task<DynamicFormWorkFlowForm> DeleteDynamicFormWorkFlowForm(DynamicFormWorkFlowForm dynamicFormWorkFlowForm);
        Task<DynamicFormDataSectionLock> UpdateDynamicFormDataSectionLock(DynamicFormDataSectionLock value);
        Task<IReadOnlyList<DynamicFormSectionAttribute>> GetDynamicFormSectionAttributeForSpinEditAsync(long? dynamicFormId);
        Task<DynamicFormSectionAttribute> UpdateFormulaTextBox(DynamicFormSectionAttribute dynamicFormSectionAttribute);
        Task<DynamicFormReportItems> InsertDynamicFormEmailSubCont(IEnumerable<DynamicFormReportItems> subjectData, IEnumerable<DynamicFormReportItems> contentData, Guid? SessionId);
        Task<IReadOnlyList<DynamicFormEmailSubCont>> GetDynamicFormEmailSubCont(Guid? SessionId);
        Task<DynamicFormEmailSubCont> DeleteDynamicFormEmailSubCont(Guid? SessionId);
        Task<DynamicForm> InsertDynamicFormPermissionPermission(DynamicForm dynamicForm);
        Task<IReadOnlyList<ApplicationPermission>> GetDynamicFormMenuList();
        Task<ApplicationPermission> UpdateDynamicFormMenuSortOrder(ApplicationPermission applicationPermission);
        Task<DynamicForm> DeleteDynamicFormMenu(DynamicForm dynamicForm);
        Task<IReadOnlyList<DynamicFormSectionAttrFormulaFunction>> GetDynamicFormSectionAttrFormulaFunction(long? DynamicFormSectionAttributeId);
        Task<IReadOnlyList<DynamicFormSectionAttrFormulaMasterFunction>> GetDynamicFormSectionAttrFormulaMasterFunction();
        Task<DynamicFormSectionAttrFormulaFunction> InsertOrUpdateDynamicFormSectionAttrFormulaFunction(DynamicFormSectionAttrFormulaFunction value);
        Task<DynamicFormSectionAttrFormulaFunction> DeleteDynamicFormSectionAttrFormulaFunction(DynamicFormSectionAttrFormulaFunction dynamicFormSectionAttrFormulaFunction);
        Task<DynamicFormDataAssignUser> InsertDynamicFormDataAssignUser(DynamicFormDataAssignUser value);
        Task<IReadOnlyList<DynamicFormDataAssignUser>> GetDynamicFormDataAssignUserList(long? DynamicFormId);
        Task<IReadOnlyList<DynamicFormDataAssignUser>> GetDynamicFormDataAssignUserAllList();
        Task<DynamicFormSectionAttribute> UpdateDynamicFormSectionAttributeGridSequenceSortOrder(DynamicFormSectionAttribute dynamicFormSectionAttribute);
        Task<DynamicFormSectionAttribute> UpdateDynamicFormSectionAttributeAllByCheckBox(DynamicFormSectionAttribute dynamicFormSectionAttribute);
        Task<long> DeleteDynamicFormDataAssignUser(long? Id, List<long?> Ids);
        Task<IReadOnlyList<DynamicFormDataAudit>> GetDynamicFormDataAuditList(Guid? sessionId);
        Task<IReadOnlyList<DynamicFormDataAudit>> GetDynamicFormDataAuditBySessionList(Guid? SessionId);
        Task<IReadOnlyList<DynamicFormFormulaMathFun>> GetDynamicFormFormulaMathFunList();
        Task<IReadOnlyList<DynamicFormSectionSecurity>> GetDynamicFormSectionSecuritySettingList(long? Id);
        Task<IReadOnlyList<DynamicFormWorkFlowApproval>> GetDynamicFormWorkFlowApprovalSettingList(long? Id);
        Task<IReadOnlyList<DynamicFormSectionAttributeSecurity>> GetDynamicFormSectionAttributeSecuritySettingList(long? Id);
        Task<IReadOnlyList<DynamicFormSectionAttributeSectionParent>> GetDynamicFormSectionAttributeSectionParentSettings(long? dynamicFormSectionAttributeId);
        Task<IReadOnlyList<DynamicFormDataAttrUpload>> GetDynamicFormDataAttrUpload(long? DynamicFormSectionAttributeId,long? DynamicFormDataId);
        Task<DynamicFormDataAttrUpload> InsertDynamicFormDataAttrUpload(List<DynamicFormDataAttrUpload> value);
        Task<DynamicFormDataUpload> GetDynamicFormDataUploadCheckValidation(long? dynamicFormDataId, long? dynamicFormSectionId);
        Task<DynamicFormDataAttrUpload> DeleteDynamicFormDataAttrUpload(DynamicFormDataAttrUpload dynamicFormDataAttrUpload);
        Task<DynamicFormData> GetDynamicFormDataOneBySessionIdAsync(Guid? SessionId);
        Task<IReadOnlyList<DynamicFormDataAudit>> GetDynamicFormDataAuditMasterList(DynamicFormDataAudit dynamicFormDataAudit);
    }

}
