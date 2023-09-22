using Application.Queries;
using Application.Response;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModel;
using Core.EntityModels;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class GetAllTransferPermissionsHandler : IRequestHandler<GetTransferPermissionsUserGroupUsers, List<UserGroupUser>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public GetAllTransferPermissionsHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<UserGroupUser>> Handle(GetTransferPermissionsUserGroupUsers request, CancellationToken cancellationToken)
        {
            return (List<UserGroupUser>)await _queryRepository.GetUserGroupUsers(request.UserIds);
        }
    }
    public class DeleteTransferPermissionsUserGroupUserHandler : IRequestHandler<DeleteTransferPermissionsUserGroupUser, List<long?>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public DeleteTransferPermissionsUserGroupUserHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<long?>> Handle(DeleteTransferPermissionsUserGroupUser request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteTransferPermissionsUserGroupUser(request.Ids);
        }
    }
    public class UpdateTransferPermissionsUserGroupUserHandler : IRequestHandler<UpdateTransferPermissionsUserGroupUser, List<long?>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public UpdateTransferPermissionsUserGroupUserHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<long?>> Handle(UpdateTransferPermissionsUserGroupUser request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateTransferPermissionsUserGroupUser(request.Ids,request.UserIds);
        }
    }


    public class GetResetPermissionDocumentUserRoleListHandler : IRequestHandler<GetResetPermissionDocumentUserRoleList, List<DocumentUserRoleModel>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public GetResetPermissionDocumentUserRoleListHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<DocumentUserRoleModel>> Handle(GetResetPermissionDocumentUserRoleList request, CancellationToken cancellationToken)
        {
            return (List<DocumentUserRoleModel>)await _queryRepository.GetTransferPermissionDocumentUserRoleList(request.Id);
        }
    }
    public class DeleteTransferPermissionsDocumentUserRoleUserHandler : IRequestHandler<DeleteTransferPermissionsDocumentUserRoleUser, List<long?>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public DeleteTransferPermissionsDocumentUserRoleUserHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<long?>> Handle(DeleteTransferPermissionsDocumentUserRoleUser request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteTransferPermissionsDocumentUserRoleUser(request.Ids);
        }
    }
    public class UpdateTransferPermissionsDocumentUserRoleUserHandler : IRequestHandler<UpdateTransferPermissionsDocumentUserRoleUser, List<long?>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public UpdateTransferPermissionsDocumentUserRoleUserHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<long?>> Handle(UpdateTransferPermissionsDocumentUserRoleUser request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateTransferPermissionsDocumentUserRoleUser(request.Ids, request.UserIds);
        }
    }
    public class GetTransferPermissionsEmailConversationParticipantHandler : IRequestHandler<GetTransferPermissionsEmailConversationParticipant, List<EmailConversations>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public GetTransferPermissionsEmailConversationParticipantHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<EmailConversations>> Handle(GetTransferPermissionsEmailConversationParticipant request, CancellationToken cancellationToken)
        {
            return (List<EmailConversations>)await _queryRepository.GetTransferPermissionsEmailConversationParticipant(request.Id);
        }
    }

    public class UpdateTransferPermissionsEmailConversationUserHandler : IRequestHandler<UpdateTransferPermissionsEmailConversationUser, EmailConversations>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public UpdateTransferPermissionsEmailConversationUserHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<EmailConversations> Handle(UpdateTransferPermissionsEmailConversationUser request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateTransferPermissionsEmailConversationParticipant(request.Ids, request.UserIds);
        }
    }
}
