using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetTransferPermissionsUserGroupUsers : PagedRequest, IRequest<List<UserGroupUser>>
    {
        public long? UserIds { get; set; }
        public GetTransferPermissionsUserGroupUsers(long? userIds)
        {
            this.UserIds = userIds;
        }
    }
    public class DeleteTransferPermissionsUserGroupUser : PagedRequest, IRequest<List<long?>>
    {
        public List<long?> Ids { get; set; }
        public DeleteTransferPermissionsUserGroupUser(List<long?> ids)
        {
            this.Ids = ids;
        }
    }
    public class UpdateTransferPermissionsUserGroupUser : PagedRequest, IRequest<List<long?>>
    {
        public List<long?> Ids { get; set; }
        public long? UserIds { get; set; }
        public UpdateTransferPermissionsUserGroupUser(List<long?> ids, long? userIds)
        {
            this.Ids = ids;
            this.UserIds = userIds;
        }
    }
    
    public class GetResetPermissionDocumentUserRoleList : PagedRequest, IRequest<List<DocumentUserRoleModel>>
    {
        public long? Id { get; set; }
        public GetResetPermissionDocumentUserRoleList(long? id)
        {
            this.Id = id;
        }
    }
    public class DeleteTransferPermissionsDocumentUserRoleUser : PagedRequest, IRequest<List<long?>>
    {
        public List<long?> Ids { get; set; }
        public DeleteTransferPermissionsDocumentUserRoleUser(List<long?> ids)
        {
            this.Ids = ids;
        }
    }
    public class UpdateTransferPermissionsDocumentUserRoleUser : PagedRequest, IRequest<List<long?>>
    {
        public List<long?> Ids { get; set; }
        public long? UserIds { get; set; }
        public UpdateTransferPermissionsDocumentUserRoleUser(List<long?> ids, long? userIds)
        {
            this.Ids = ids;
            this.UserIds = userIds;
        }
    }
    public class GetTransferPermissionsEmailConversationParticipant : PagedRequest, IRequest<List<EmailConversations>>
    {
        public long? Id { get; set; }
        public GetTransferPermissionsEmailConversationParticipant(long? id)
        {
            this.Id = id;
        }
    }
    public class UpdateTransferPermissionsEmailConversationUser : PagedRequest, IRequest<EmailConversations>
    {
        public List<EmailConversations> Ids { get; set; }
        public long? FromUserId { get; set; }
        public long? ToUserId { get; set; }
        public long? UserId { get; set; }
        public UpdateTransferPermissionsEmailConversationUser(List<EmailConversations> ids, long? fromid, long? toid, long? userId)
        {
            this.Ids = ids;
            this.FromUserId = fromid;
            this.ToUserId = toid;
            this.UserId = userId;
            
        }
    }
    public class GetTransferDynamicFormWorkFlow : PagedRequest, IRequest<List<DynamicFormWorkFlow>>
    {
        public long? UserIds { get; set; }
        public GetTransferDynamicFormWorkFlow(long? userIds)
        {
            this.UserIds = userIds;
        }
    }
    public class UpdateTransferDynamicFormWorkFlow : PagedRequest, IRequest<List<long?>>
    {
        public List<long?> Ids { get; set; }
        public long? UserIds { get; set; }
        public UpdateTransferDynamicFormWorkFlow(List<long?> ids, long? userIds)
        {
            this.Ids = ids;
            this.UserIds = userIds;
        }
    }

    public class GetTransferDynamicFormWorkFlowApproval : PagedRequest, IRequest<List<DynamicFormWorkFlowApproval>>
    {
        public long? UserIds { get; set; }
        public GetTransferDynamicFormWorkFlowApproval(long? userIds)
        {
            this.UserIds = userIds;
        }
    }
    public class UpdateTransferDynamicFormWorkFlowApproval : PagedRequest, IRequest<List<long?>>
    {
        public List<long?> Ids { get; set; }
        public long? UserIds { get; set; }
        public UpdateTransferDynamicFormWorkFlowApproval(List<long?> ids, long? userIds)
        {
            this.Ids = ids;
            this.UserIds = userIds;
        }
    }
    public class GetTransferDynamicFormSectionSecurity : PagedRequest, IRequest<List<DynamicFormSectionSecurity>>
    {
        public long? UserIds { get; set; }
        public GetTransferDynamicFormSectionSecurity(long? userIds)
        {
            this.UserIds = userIds;
        }
    }
    public class UpdateTransferDynamicFormSectionSecurity : PagedRequest, IRequest<List<long?>>
    {
        public List<long?> Ids { get; set; }
        public long? UserIds { get; set; }
        public UpdateTransferDynamicFormSectionSecurity(List<long?> ids, long? userIds)
        {
            this.Ids = ids;
            this.UserIds = userIds;
        }
    }
    public class DeleteTransferDynamicFormSectionSecurity : PagedRequest, IRequest<List<long?>>
    {
        public List<long?> Ids { get; set; }
        public DeleteTransferDynamicFormSectionSecurity(List<long?> ids)
        {
            this.Ids = ids;
        }
    }

    public class GetTransferDynamicFormSectionAttributeSecurity : PagedRequest, IRequest<List<DynamicFormSectionAttributeSecurity>>
    {
        public long? UserIds { get; set; }
        public GetTransferDynamicFormSectionAttributeSecurity(long? userIds)
        {
            this.UserIds = userIds;
        }
    }
    public class UpdateTransferDynamicFormSectionAttributeSecurity : PagedRequest, IRequest<List<long?>>
    {
        public List<long?> Ids { get; set; }
        public long? UserIds { get; set; }
        public UpdateTransferDynamicFormSectionAttributeSecurity(List<long?> ids, long? userIds)
        {
            this.Ids = ids;
            this.UserIds = userIds;
        }
    }
    public class DeleteTransferDynamicFormSectionAttributeSecurity : PagedRequest, IRequest<List<long?>>
    {
        public List<long?> Ids { get; set; }
        public DeleteTransferDynamicFormSectionAttributeSecurity(List<long?> ids)
        {
            this.Ids = ids;
        }
    }

    public class GetTransferDynamicFormSectionAttributeSection : PagedRequest, IRequest<List<DynamicFormSectionAttributeSection>>
    {
        public long? UserIds { get; set; }
        public GetTransferDynamicFormSectionAttributeSection(long? userIds)
        {
            this.UserIds = userIds;
        }
    }
    public class UpdateDynamicFormSectionAttributeSection : PagedRequest, IRequest<List<long?>>
    {
        public List<long?> Ids { get; set; }
        public long? UserIds { get; set; }
        public UpdateDynamicFormSectionAttributeSection(List<long?> ids, long? userIds)
        {
            this.Ids = ids;
            this.UserIds = userIds;
        }
    }
    public class DeleteDynamicFormSectionAttributeSection : PagedRequest, IRequest<List<DynamicFormSectionAttributeSection>>
    {
        public List<DynamicFormSectionAttributeSection> Ids { get; set; }
        public DeleteDynamicFormSectionAttributeSection(List<DynamicFormSectionAttributeSection> ids)
        {
            this.Ids = ids;
        }
    }


    public class GetTransferDynamicFormApproval : PagedRequest, IRequest<List<DynamicFormApproval>>
    {
        public long? UserIds { get; set; }
        public GetTransferDynamicFormApproval(long? userIds)
        {
            this.UserIds = userIds;
        }
    }
    public class DeleteTransferDynamicFormApproval : PagedRequest, IRequest<List<DynamicFormApproval>>
    {
        public List<DynamicFormApproval> Ids { get; set; }
        public DeleteTransferDynamicFormApproval(List<DynamicFormApproval> ids)
        {
            this.Ids = ids;
        }
    }
    public class UpdateTransferDynamicFormApproval : PagedRequest, IRequest<List<long?>>
    {
        public List<long?> Ids { get; set; }
        public long? UserIds { get; set; }
        public UpdateTransferDynamicFormApproval(List<long?> ids,long? userIds)
        {
            this.Ids = ids;
            this.UserIds = userIds;
        }
    }
}
