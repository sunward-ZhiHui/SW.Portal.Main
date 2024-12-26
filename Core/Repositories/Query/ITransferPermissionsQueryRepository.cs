
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface ITransferPermissionsQueryRepository : IQueryRepository<DocumentUserRoleModel>
    {
        Task<IReadOnlyList<UserGroupUser>> GetUserGroupUsers(long? userIds);
        Task<List<long?>> DeleteTransferPermissionsUserGroupUser(List<long?> ids);
        Task<List<long?>> UpdateTransferPermissionsUserGroupUser(List<long?> ids, long? Id);
        Task<IReadOnlyList<DocumentUserRoleModel>> GetTransferPermissionDocumentUserRoleList(long? Id);
        Task<List<long?>> DeleteTransferPermissionsDocumentUserRoleUser(List<long?> ids);
        Task<List<long?>> UpdateTransferPermissionsDocumentUserRoleUser(List<long?> ids, long? Id);
        Task<IReadOnlyList<EmailConversations>> GetTransferPermissionsEmailConversationParticipant(long? userIds);
        Task<EmailConversations> UpdateTransferPermissionsEmailConversationParticipant(List<EmailConversations> ids, long? userId);
        Task InsertEmailTransferHistory(long fromUserId, long toUserId, long emailConversationId, long topicId, long addedByUserId);
        Task<IReadOnlyList<DynamicFormWorkFlow>> GetTransferDynamicFormWorkFlow(long? userIds);
        Task<List<long?>> UpdateTransferDynamicFormWorkFlow(List<long?> ids, long? userId);
        Task<IReadOnlyList<DynamicFormWorkFlowApproval>> GetTransferDynamicFormWorkFlowApproval(long? userIds);
        Task<List<long?>> UpdateTransferDynamicFormWorkFlowApproval(List<long?> ids, long? userId);
        Task<IReadOnlyList<DynamicFormSectionSecurity>> GetTransferDynamicFormSectionSecurity(long? userIds);
        Task<List<long?>> UpdateTransferDynamicFormSectionSecurity(List<long?> ids, long? userId);
        Task<List<long?>> DeleteTransferDynamicFormSectionSecurity(List<long?> ids);
        Task<IReadOnlyList<DynamicFormSectionAttributeSecurity>> GetTransferDynamicFormSectionAttributeSecurity(long? userIds);
        Task<List<long?>> UpdateTransferDynamicFormSectionAttributeSecurity(List<long?> ids, long? userId);
        Task<List<long?>> DeleteTransferDynamicFormSectionAttributeSecurity(List<long?> ids);
        Task<IReadOnlyList<DynamicFormSectionAttributeSection>> GetTransferDynamicFormSectionAttributeSection(long? userIds);
        Task<List<long?>> UpdateDynamicFormSectionAttributeSection(List<long?> ids, long? userId);
        Task<List<DynamicFormSectionAttributeSection>> DeleteDynamicFormSectionAttributeSection(List<DynamicFormSectionAttributeSection> ids);
        Task<IReadOnlyList<DynamicFormApproval>> GetTransferDynamicFormApproval(long? userIds);
        Task<List<DynamicFormApproval>> DeleteTransferDynamicFormApproval(List<DynamicFormApproval> ids);
        Task<List<long?>> UpdateTransferDynamicFormApproval(List<long?> ids, long? userId);
        Task<IReadOnlyList<DynamicFormApproved>> GetTransferDynamicFormApproved(long? userIds);
        Task<DynamicFormApproved> UpdateTransferDynamicFormApproved(List<DynamicFormApproved> ids, long? userIds);
        Task<IReadOnlyList<DynamicFormWorkFlowForm>> GetTransferDynamicFormWorkFlowForm(long? userIds);
        Task<DynamicFormWorkFlowForm> UpdateTransferDynamicFormDataWorkFlowForm(List<DynamicFormWorkFlowForm> ids, long? userIds);
        Task<IReadOnlyList<DynamicFormData>> GetTransferDynamicFormDataLock(long? userIds);
        Task<List<long?>> UpdateTransferDynamicFormDataLock(List<long?> ids, long? userId);
        Task<IReadOnlyList<DynamicFormDataSectionLock>> GetTransferDynamicFormDataSectionLock(long? userIds);
        Task<List<long?>> UpdateTransferDynamicFormDataSectionLock(List<long?> ids, long? userId);
        Task<List<long?>> UpdateTransferDynamicFormDataSectionLockLock(List<long?> ids);
        Task<IReadOnlyList<DynamicFormWorkFlowApprovedForm>> GetTransferDynamicFormWorkFlowFormApproved(long? userIds);
        Task<DynamicFormWorkFlowApprovedForm> UpdateTransferDynamicFormDataWorkFlowFormApproved(List<DynamicFormWorkFlowApprovedForm> ids, long? userIds);
        Task<List<long?>> UpdateTransferDynamicFormDataLockLock(List<long?> ids);
    }
}
