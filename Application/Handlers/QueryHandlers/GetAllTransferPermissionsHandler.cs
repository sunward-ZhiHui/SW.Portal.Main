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
            return await _queryRepository.UpdateTransferPermissionsUserGroupUser(request.Ids, request.UserIds);
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
            var emailConversations = await _queryRepository.UpdateTransferPermissionsEmailConversationParticipant(request.Ids, request.ToUserId);

            //foreach (var conversation in request.Ids)
            //{
            //    if (conversation != null)
            //    {
            //        await _queryRepository.InsertEmailTransferHistory(
            //            fromUserId: request.FromUserId.Value,
            //            toUserId: request.ToUserId.Value,
            //            emailConversationId: conversation.ConversationId.Value,
            //            topicId: conversation.TopicID,
            //            addedByUserId: request.UserId.Value
            //        );
            //    }
            //}


            return emailConversations;
        }
    }
    public class GetTransferDynamicFormWorkFlowHandler : IRequestHandler<GetTransferDynamicFormWorkFlow, List<DynamicFormWorkFlow>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public GetTransferDynamicFormWorkFlowHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<DynamicFormWorkFlow>> Handle(GetTransferDynamicFormWorkFlow request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormWorkFlow>)await _queryRepository.GetTransferDynamicFormWorkFlow(request.UserIds);
        }
    }
    public class UpdateTransferDynamicFormWorkFlowHandler : IRequestHandler<UpdateTransferDynamicFormWorkFlow, List<long?>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public UpdateTransferDynamicFormWorkFlowHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<long?>> Handle(UpdateTransferDynamicFormWorkFlow request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateTransferDynamicFormWorkFlow(request.Ids, request.UserIds);
        }
    }

    public class GetTransferDynamicFormWorkFlowApprovalHandler : IRequestHandler<GetTransferDynamicFormWorkFlowApproval, List<DynamicFormWorkFlowApproval>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public GetTransferDynamicFormWorkFlowApprovalHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<DynamicFormWorkFlowApproval>> Handle(GetTransferDynamicFormWorkFlowApproval request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormWorkFlowApproval>)await _queryRepository.GetTransferDynamicFormWorkFlowApproval(request.UserIds);
        }
    }
    public class UpdateTransferDynamicFormWorkFlowApprovalHandler : IRequestHandler<UpdateTransferDynamicFormWorkFlowApproval, List<long?>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public UpdateTransferDynamicFormWorkFlowApprovalHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<long?>> Handle(UpdateTransferDynamicFormWorkFlowApproval request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateTransferDynamicFormWorkFlowApproval(request.Ids, request.UserIds);
        }
    }

    public class GetTransferDynamicFormSectionSecurityHandler : IRequestHandler<GetTransferDynamicFormSectionSecurity, List<DynamicFormSectionSecurity>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public GetTransferDynamicFormSectionSecurityHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<DynamicFormSectionSecurity>> Handle(GetTransferDynamicFormSectionSecurity request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormSectionSecurity>)await _queryRepository.GetTransferDynamicFormSectionSecurity(request.UserIds);
        }
    }
    public class UpdateTransferDynamicFormSectionSecurityHandler : IRequestHandler<UpdateTransferDynamicFormSectionSecurity, List<long?>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public UpdateTransferDynamicFormSectionSecurityHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<long?>> Handle(UpdateTransferDynamicFormSectionSecurity request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateTransferDynamicFormSectionSecurity(request.Ids, request.UserIds);
        }
    }
    public class DeleteTransferDynamicFormSectionSecurityHandler : IRequestHandler<DeleteTransferDynamicFormSectionSecurity, List<long?>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public DeleteTransferDynamicFormSectionSecurityHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<long?>> Handle(DeleteTransferDynamicFormSectionSecurity request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteTransferDynamicFormSectionSecurity(request.Ids);
        }
    }


    public class GetTransferDynamicFormSectionAttributeSecurityHandler : IRequestHandler<GetTransferDynamicFormSectionAttributeSecurity, List<DynamicFormSectionAttributeSecurity>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public GetTransferDynamicFormSectionAttributeSecurityHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<DynamicFormSectionAttributeSecurity>> Handle(GetTransferDynamicFormSectionAttributeSecurity request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormSectionAttributeSecurity>)await _queryRepository.GetTransferDynamicFormSectionAttributeSecurity(request.UserIds);
        }
    }
    public class UpdateTransferDynamicFormSectionAttributeSecurityHandler : IRequestHandler<UpdateTransferDynamicFormSectionAttributeSecurity, List<long?>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public UpdateTransferDynamicFormSectionAttributeSecurityHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<long?>> Handle(UpdateTransferDynamicFormSectionAttributeSecurity request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateTransferDynamicFormSectionAttributeSecurity(request.Ids, request.UserIds);
        }
    }
    public class DeleteTransferDynamicFormSectionAttributeSecurityHandler : IRequestHandler<DeleteTransferDynamicFormSectionAttributeSecurity, List<long?>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public DeleteTransferDynamicFormSectionAttributeSecurityHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<long?>> Handle(DeleteTransferDynamicFormSectionAttributeSecurity request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteTransferDynamicFormSectionAttributeSecurity(request.Ids);
        }
    }

    public class GetTransferDynamicFormSectionAttributeSectionHandler : IRequestHandler<GetTransferDynamicFormSectionAttributeSection, List<DynamicFormSectionAttributeSection>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public GetTransferDynamicFormSectionAttributeSectionHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<DynamicFormSectionAttributeSection>> Handle(GetTransferDynamicFormSectionAttributeSection request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormSectionAttributeSection>)await _queryRepository.GetTransferDynamicFormSectionAttributeSection(request.UserIds);
        }
    }
    public class UpdateDynamicFormSectionAttributeSectionHandler : IRequestHandler<UpdateDynamicFormSectionAttributeSection, List<long?>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public UpdateDynamicFormSectionAttributeSectionHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<long?>> Handle(UpdateDynamicFormSectionAttributeSection request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateDynamicFormSectionAttributeSection(request.Ids, request.UserIds);
        }
    }
    public class DeleteDynamicFormSectionAttributeSectionHandler : IRequestHandler<DeleteDynamicFormSectionAttributeSection, List<DynamicFormSectionAttributeSection>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public DeleteDynamicFormSectionAttributeSectionHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<DynamicFormSectionAttributeSection>> Handle(DeleteDynamicFormSectionAttributeSection request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteDynamicFormSectionAttributeSection(request.Ids);
        }
    }

    public class GetTransferDynamicFormApprovalHandler : IRequestHandler<GetTransferDynamicFormApproval, List<DynamicFormApproval>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public GetTransferDynamicFormApprovalHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<DynamicFormApproval>> Handle(GetTransferDynamicFormApproval request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormApproval>)await _queryRepository.GetTransferDynamicFormApproval(request.UserIds);
        }
    }
    public class UpdateTransferDynamicFormApprovalHandler : IRequestHandler<UpdateTransferDynamicFormApproval, List<long?>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public UpdateTransferDynamicFormApprovalHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<long?>> Handle(UpdateTransferDynamicFormApproval request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateTransferDynamicFormApproval(request.Ids, request.UserIds);
        }
    }
    public class DeleteTransferDynamicFormApprovalHandler : IRequestHandler<DeleteTransferDynamicFormApproval, List<DynamicFormApproval>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public DeleteTransferDynamicFormApprovalHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<DynamicFormApproval>> Handle(DeleteTransferDynamicFormApproval request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteTransferDynamicFormApproval(request.Ids);
        }
    }


    public class GetTransferDynamicFormApprovedHandler : IRequestHandler<GetTransferDynamicFormApproved, List<DynamicFormApproved>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public GetTransferDynamicFormApprovedHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<DynamicFormApproved>> Handle(GetTransferDynamicFormApproved request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormApproved>)await _queryRepository.GetTransferDynamicFormApproved(request.UserIds);
        }
    }
    public class UpdateTransferDynamicFormApprovedHandler : IRequestHandler<UpdateTransferDynamicFormApproved, DynamicFormApproved>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public UpdateTransferDynamicFormApprovedHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<DynamicFormApproved> Handle(UpdateTransferDynamicFormApproved request, CancellationToken cancellationToken)
        {
            return  await _queryRepository.UpdateTransferDynamicFormApproved(request.Ids, request.UserIds);
        }
    }

    public class GetTransferDynamicFormWorkFlowFormHandler : IRequestHandler<GetTransferDynamicFormWorkFlowForm, List<DynamicFormWorkFlowForm>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public GetTransferDynamicFormWorkFlowFormHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<DynamicFormWorkFlowForm>> Handle(GetTransferDynamicFormWorkFlowForm request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormWorkFlowForm>)await _queryRepository.GetTransferDynamicFormWorkFlowForm(request.UserIds);
        }
    }
    public class UpdateTransferDynamicFormDataWorkFlowFormHandler : IRequestHandler<UpdateTransferDynamicFormDataWorkFlowForm, DynamicFormWorkFlowForm>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public UpdateTransferDynamicFormDataWorkFlowFormHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<DynamicFormWorkFlowForm> Handle(UpdateTransferDynamicFormDataWorkFlowForm request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateTransferDynamicFormDataWorkFlowForm(request.Ids, request.UserIds);
        }
    }

    public class GetTransferDynamicFormDataLockHandler : IRequestHandler<GetTransferDynamicFormDataLock, List<DynamicFormData>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public GetTransferDynamicFormDataLockHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<DynamicFormData>> Handle(GetTransferDynamicFormDataLock request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormData>)await _queryRepository.GetTransferDynamicFormDataLock(request.UserIds);
        }
    }
    public class UpdateTransferDynamicFormDataLockHandler : IRequestHandler<UpdateTransferDynamicFormDataLock, List<long?>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public UpdateTransferDynamicFormDataLockHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<long?>> Handle(UpdateTransferDynamicFormDataLock request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateTransferDynamicFormDataLock(request.Ids, request.UserIds);
        }
    }

    public class GetTransferDynamicFormDataSectionLockHandler : IRequestHandler<GetTransferDynamicFormDataSectionLock, List<DynamicFormDataSectionLock>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public GetTransferDynamicFormDataSectionLockHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<DynamicFormDataSectionLock>> Handle(GetTransferDynamicFormDataSectionLock request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormDataSectionLock>)await _queryRepository.GetTransferDynamicFormDataSectionLock(request.UserIds);
        }
    }
    public class UpdateTransferDynamicFormDataSectionLockHandler : IRequestHandler<UpdateTransferDynamicFormDataSectionLock, List<long?>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public UpdateTransferDynamicFormDataSectionLockHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<long?>> Handle(UpdateTransferDynamicFormDataSectionLock request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateTransferDynamicFormDataSectionLock(request.Ids, request.UserIds);
        }
    }
    public class UpdateTransferDynamicFormDataSectionLockLockHandler : IRequestHandler<UpdateTransferDynamicFormDataSectionLockLock, List<long?>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public UpdateTransferDynamicFormDataSectionLockLockHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<long?>> Handle(UpdateTransferDynamicFormDataSectionLockLock request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateTransferDynamicFormDataSectionLockLock(request.Ids);
        }
    }

    public class GetTransferDynamicFormWorkFlowFormApprovedHandler : IRequestHandler<GetTransferDynamicFormWorkFlowFormApproved, List<DynamicFormWorkFlowApprovedForm>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public GetTransferDynamicFormWorkFlowFormApprovedHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<DynamicFormWorkFlowApprovedForm>> Handle(GetTransferDynamicFormWorkFlowFormApproved request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormWorkFlowApprovedForm>)await _queryRepository.GetTransferDynamicFormWorkFlowFormApproved(request.UserIds);
        }
    }
    public class UpdateTransferDynamicFormDataWorkFlowFormApprovedHandler : IRequestHandler<UpdateTransferDynamicFormDataWorkFlowFormApproved, DynamicFormWorkFlowApprovedForm>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public UpdateTransferDynamicFormDataWorkFlowFormApprovedHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<DynamicFormWorkFlowApprovedForm> Handle(UpdateTransferDynamicFormDataWorkFlowFormApproved request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateTransferDynamicFormDataWorkFlowFormApproved(request.Ids, request.UserIds);
        }
    }
    public class UpdateTransferDynamicFormDataLockLockHandler : IRequestHandler<UpdateTransferDynamicFormDataLockLock, List<long?>>
    {
        private readonly ITransferPermissionsQueryRepository _queryRepository;
        public UpdateTransferDynamicFormDataLockLockHandler(ITransferPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<long?>> Handle(UpdateTransferDynamicFormDataLockLock request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateTransferDynamicFormDataLockLock(request.Ids);
        }
    }
}
