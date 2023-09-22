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
        public long? UserIds { get; set; }
        public UpdateTransferPermissionsEmailConversationUser(List<EmailConversations> ids, long? userIds)
        {
            this.Ids = ids;
            this.UserIds = userIds;
        }
    }

}
