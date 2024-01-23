using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IDynamicFormQueryRepository : IQueryRepository<DynamicForm>
    {
        Task<DynamicForm> GetAllSelectedList(Guid? sessionId,long? DynamicFormDataId);
        Task<IReadOnlyList<DynamicForm>> GetAllAsync(long? userId);
        Task<DynamicForm> GetDynamicFormByIdAsync(long? Id);
        Task<DynamicForm> GetDynamicFormBySessionIdAsync(Guid? SessionId);
        Task<long> Insert(DynamicForm dynamicForm);
        Task<long> Update(DynamicForm dynamicForm);
        Task<long> Delete(long id);
        DynamicForm GetDynamicFormScreenNameCheckValidation(string? value, long id);
        Task<DynamicFormSection> InsertOrUpdateDynamicFormSection(DynamicFormSection dynamicFormSection);
        Task<IReadOnlyList<DynamicFormSection>> GetDynamicFormSectionAsync(long? dynamicFormId);
        Task<DynamicFormSectionAttribute> InsertOrUpdateDynamicFormSectionAttribute(DynamicFormSectionAttribute dynamicFormSectionAttribute);
        Task<IReadOnlyList<DynamicFormSectionAttribute>> GetDynamicFormSectionAttributeAsync(long? dynamicFormSectionId);
        Task<long> DeleteDynamicFormSection(DynamicFormSection dynamicFormSection);
        Task<DynamicFormSection> UpdateDynamicFormSectionSortOrder(DynamicFormSection dynamicFormSection);
        Task<long> DeleteDynamicFormSectionAttribute(DynamicFormSectionAttribute dynamicFormSectionAttribute);
        Task<DynamicFormSectionAttribute> UpdateDynamicFormSectionAttributeSortOrder(DynamicFormSectionAttribute dynamicFormSectionAttribute);
        Task<long> InsertDynamicFormAttributeSection(long dynamicFormSectionId, IEnumerable<AttributeHeader> attributeIds, long? userId);
        Task<DynamicFormData> InsertOrUpdateDynamicFormData(DynamicFormData dynamicFormData);
        Task<DynamicFormData> GetDynamicFormDataBySessionIdAsync(Guid? SessionId);
        Task<DocumentsModel> GetDynamicFormDataBySessionIdForDMSAsync(Guid? SessionId);
        Task<IReadOnlyList<DynamicFormData>> GetDynamicFormDataByIdAsync(long? id,long? userId);
        Task<DynamicFormData> DeleteDynamicFormData(DynamicFormData dynamicFormData);
        Task<IReadOnlyList<DynamicFormApproval>> GetDynamicFormApprovalAsync(long? dynamicFormId);
        Task<DynamicFormApproval> InsertOrUpdateDynamicFormApproval(DynamicFormApproval dynamicFormApproval);
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
        Task<IReadOnlyList<DynamicFormSection>> GetDynamicFormSectionByIdAsync(long? id, long? UserId,long? dynamicFormDataId);
        Task<DynamicFormDataUpload> InsertDynamicFormDataUpload(DynamicFormDataUpload dynamicFormSection);
    }

}
