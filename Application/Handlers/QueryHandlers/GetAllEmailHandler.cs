using Application.Common.Mapper;
using Application.Queries;
using Application.Response;
using Core.Entities;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;
using System.Net.Mime;

namespace CMS.Application.Handlers.QueryHandlers
{
   
    public class GetUserEmailTopicListHandler : IRequestHandler<GetUserEmailTopics, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetUserEmailTopicListHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
           _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetUserEmailTopics request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetUserTopicList(request.UserId);
           
        }
    }
    public class UpdateTopicArchiveHandler : IRequestHandler<UpdateTopicArchive, long>
    {
        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;

        public UpdateTopicArchiveHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;

        }
        public async Task<long> Handle(UpdateTopicArchive request, CancellationToken cancellationToken)
        {
            var req = await _emailTopicsQueryRepository.UpdateTopicArchive(request);
            return req;
        }
    }
    public class UpdateTopicGroupArchiveHandler : IRequestHandler<UpdateTopicGroupArchive, long>
    {
        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;

        public UpdateTopicGroupArchiveHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;

        }
        public async Task<long> Handle(UpdateTopicGroupArchive request, CancellationToken cancellationToken)
        {
            var req = await _emailTopicsQueryRepository.UpdateTopicGroupArchive(request);
            return req;
        }
    }

    public class UpdateTopicUnArchiveHandler : IRequestHandler<UpdateTopicUnArchive, long>
    {
        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;

        public UpdateTopicUnArchiveHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;

        }
        public async Task<long> Handle(UpdateTopicUnArchive request, CancellationToken cancellationToken)
        {
            var req = await _emailTopicsQueryRepository.UpdateTopicUnArchive(request);
            return req;
        }
    }
    public class GetListBySessionHandler : IRequestHandler<GetListBySession, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetListBySessionHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetListBySession request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetBySessionTopicList(request.SessionId);

        }
    }
    public class GetListByIdListHandler : IRequestHandler<GetListByIdList, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetListByIdListHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetListByIdList request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetByIdTopicListAsync(request.ID);

        }
    }
    
    public class GetEmailTopicToHandler : IRequestHandler<GetEmailTopicTo, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetEmailTopicToHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetEmailTopicTo request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetTopicToList(request.UserId,request.SearchTxt);
        }
    }
    public class GetEmailTopicToSearchHandler : IRequestHandler<GetEmailTopicToSearch, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetEmailTopicToSearchHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetEmailTopicToSearch request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetTopicToSearchList(request.SearchTxt,request.UserId);
        }
    }
    public class SetPinEmailTopicToHandler : IRequestHandler<SetPinEmailTopicTo, long>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public SetPinEmailTopicToHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<long> Handle(SetPinEmailTopicTo request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.SetPinTopicToList(request.ID);

        }
    }
    public class UnSetPinEmailTopicToHandler : IRequestHandler<UnSetPinEmailTopicTo, long>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public UnSetPinEmailTopicToHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<long> Handle(UnSetPinEmailTopicTo request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.UnSetPinTopicToList(request.ID);

        }
    }
    public class UpdateMarkasAllReadHandler : IRequestHandler<UpdateMarkasAllRead, long>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public UpdateMarkasAllReadHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<long> Handle(UpdateMarkasAllRead request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.UpdateMarkasAllReadList(request.ID,request.UserId);

        }
    }
    public class UpdateMarkasReadHandler : IRequestHandler<UpdateMarkasRead, long>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public UpdateMarkasReadHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<long> Handle(UpdateMarkasRead request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.UpdateMarkasReadList(request.ID);

        }
    }
    public class UpdateMarkasunReadHandler : IRequestHandler<UpdateMarkasunRead, long>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public UpdateMarkasunReadHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<long> Handle(UpdateMarkasunRead request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.UpdateMarkasunReadList(request.ID);

        }
    }

    public class GetEmailTopicAllHandler : IRequestHandler<GetEmailTopicAll, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetEmailTopicAllHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetEmailTopicAll request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetTopicAllList(request.UserId,request.SearchTxt);

        }
    }

    public class GetEmailTopicHomeHandler : IRequestHandler<GetEmailTopicHome, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetEmailTopicHomeHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetEmailTopicHome request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetTopicHomeList(request.UserId);

        }
    }

    public class GetEmailMasterSearchAllHandler : IRequestHandler<GetEmailMasterSearchAll, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetEmailMasterSearchAllHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetEmailMasterSearchAll request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetTopicMasterSearchList(request);

        }
    }
    public class GetEmailAdminSearchAllHandler : IRequestHandler<GetEmailAdminSearchAll, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetEmailAdminSearchAllHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetEmailAdminSearchAll request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetTopicAdminSearchList(request);

        }
    }
    public class GetEmailTopicCCHandler : IRequestHandler<GetEmailTopicCC, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetEmailTopicCCHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetEmailTopicCC request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetTopicCCList(request.UserId,request.SearchTxt);

        }
    }
    public class GetByIdEmailTopicToHandler : IRequestHandler<GetByIdEmailTopicTo, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetByIdEmailTopicToHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetByIdEmailTopicTo request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetByIdTopicToList(request.ID);

        }
    }
    public class GetByIdEmailTopicCCHandler : IRequestHandler<GetByIdEmailTopicCC, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetByIdEmailTopicCCHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetByIdEmailTopicCC request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetByIdTopicCCList(request.ID);

        }
    }
    public class GetByIdEmailUserGroupToHandler : IRequestHandler<GetByIdEmailUserGroupTo, List<long>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetByIdEmailUserGroupToHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<long>> Handle(GetByIdEmailUserGroupTo request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetByIdUserGroupToList(request.ID);

        }
    }
    public class GetByIdEmailUserGroupCCHandler : IRequestHandler<GetByIdEmailUserGroupCC, List<long>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetByIdEmailUserGroupCCHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<long>> Handle(GetByIdEmailUserGroupCC request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetByIdUserGroupCCList(request.ID);

        }
    }
    public class GetEmailTopicDraftHandler : IRequestHandler<GetEmailTopicDraft, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetEmailTopicDraftHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetEmailTopicDraft request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetTopicDraftList(request.UserId);

        }
    }
    public class DeleteEmailTopicDraftHandler : IRequestHandler<DeleteEmailTopicDraft, long>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public DeleteEmailTopicDraftHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<long> Handle(DeleteEmailTopicDraft request, CancellationToken cancellationToken)
        {
            return _emailTopicsQueryRepository.DeleteTopicDraftList(request.TopicId);
        }
    }
    public class GetSubEmailTopicToHandler : IRequestHandler<GetSubEmailTopicTo, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetSubEmailTopicToHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetSubEmailTopicTo request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetSubTopicToList(request.TopicId,request.UserId, request.SearchTxt);

        }
    }
    public class GetSubEmailSearchAllHandler : IRequestHandler<GetSubEmailSearchAll, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetSubEmailSearchAllHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetSubEmailSearchAll request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetSubTopicSearchAllList(request.TopicId, request.UserId,request.SearchTxt);

        }
    }
    

    public class GetSubEmailTopicAllHandler : IRequestHandler<GetSubEmailTopicAll, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetSubEmailTopicAllHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetSubEmailTopicAll request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetSubTopicAllList(request.TopicId, request.UserId,request.SearchTxt);

        }
    }
    public class GetSubAdminEmailTopicAllHandler : IRequestHandler<GetSubAdminEmailAll, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetSubAdminEmailTopicAllHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetSubAdminEmailAll request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetSubAdminEmailTopicAllList(request.TopicId, request.UserId, request.SearchTxt);

        }
    }
    public class GetSubEmailTopicHomeHandler : IRequestHandler<GetSubEmailTopicHome, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetSubEmailTopicHomeHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetSubEmailTopicHome request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetSubTopicHomeList(request.TopicId, request.UserId);

        }
    }
    public class GetSubEmailTopicCCHandler : IRequestHandler<GetSubEmailTopicCC, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetSubEmailTopicCCHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetSubEmailTopicCC request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetSubTopicCCList(request.TopicId, request.UserId, request.SearchTxt);

        }
    }
    public class GetSubEmailTopicSentHandler : IRequestHandler<GetSubEmailTopicSent, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetSubEmailTopicSentHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetSubEmailTopicSent request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetSubTopicSentList(request.TopicId, request.UserId, request.SearchTxt);

        }
    }
   
    public class GetSentTopicHandler : IRequestHandler<GetSentTopic, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetSentTopicHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetSentTopic request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetTopicSentList(request.UserId,request.SearchTxt);

        }
    }

    public class GetEmailParticipantListHandler : IRequestHandler<GetEmailParticipantsList, List<EmailParticipant>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetEmailParticipantListHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailParticipant>> Handle(GetEmailParticipantsList request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetParticipantList(request.TopicId,request.UserId);
        }
    }
    public class GetEmailGroupParticipantListHandler : IRequestHandler<GetEmailGroupParticipantsList, List<EmailConversationAssignToUserGroup>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetEmailGroupParticipantListHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailConversationAssignToUserGroup>> Handle(GetEmailGroupParticipantsList request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetGroupParticipantList(request.TopicId, request.UserId);
        }
    }
    public class GetConversationParticipantListHandler : IRequestHandler<GetConversationParticipantsList, List<EmailParticipant>>
    {
        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetConversationParticipantListHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailParticipant>> Handle(GetConversationParticipantsList request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetConversationPList(request.ConversationId);
        }
    }
    public class GetConversationGroupParticipantListHandler : IRequestHandler<GetConversationGroupParticipantsList, List<EmailConversationAssignToUserGroup>>
    {
        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetConversationGroupParticipantListHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailConversationAssignToUserGroup>> Handle(GetConversationGroupParticipantsList request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetConversationGroupPList(request.ConversationId);
        }
    }
    public class GetParticipantbysessionidListHandler : IRequestHandler<GetParticipantbysessionidList, List<EmailParticipant>>
    {
        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetParticipantbysessionidListHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailParticipant>> Handle(GetParticipantbysessionidList request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetParticipantbysessionidList(request.sessionId);
        }
    }
    

    public class GetByIdEmailTopicListHandler : IRequestHandler<GetByIdEmailTopics, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetByIdEmailTopicListHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetByIdEmailTopics request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetByIdAsync(request.ID);

        }
    }
    
    public class CreateEmailTopicsHandler : IRequestHandler<CreateEmailTopics, long>
    {
        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;

        public CreateEmailTopicsHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<long> Handle(CreateEmailTopics request, CancellationToken cancellationToken)
        {
            var customerEntity = RoleMapper.Mapper.Map<EmailTopics>(request);

            if (customerEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var newTopics = _emailTopicsQueryRepository.Insert(customerEntity);
            var customerResponse = RoleMapper.Mapper.Map<long>(newTopics);
            return customerResponse;
        }
    }

    public class EditEmailTopicsHandler : IRequestHandler<EditEmailTopics, long>
    {
        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;

        public EditEmailTopicsHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<long> Handle(EditEmailTopics request, CancellationToken cancellationToken)
        {
            var customerEntity = RoleMapper.Mapper.Map<EmailTopics>(request);

            if (customerEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var newTopics = _emailTopicsQueryRepository.EmailTopicUpdate(customerEntity);
            var customerResponse = RoleMapper.Mapper.Map<long>(newTopics);
            return customerResponse;
        }
    }
    public class UpdateEmailTopicDueDateHandler : IRequestHandler<UpdateEmailTopicDueDate, long>
    {
        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;

        public UpdateEmailTopicDueDateHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;

        }

        public async Task<long> Handle(UpdateEmailTopicDueDate request, CancellationToken cancellationToken)
        {
            var req = await _emailTopicsQueryRepository.UpdateDueDate(request);
            return req;
        }
    }
    public class UpdateUserTagHandler : IRequestHandler<UpdateUserTag, long>
    {
        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;

        public UpdateUserTagHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }

        public async Task<long> Handle(UpdateUserTag request, CancellationToken cancellationToken)
        {
            var req = await _emailTopicsQueryRepository.UpdateUserTagAsync(request);
            return req;
        }
    }
    public class CreateUserTagHandler : IRequestHandler<CreateUserTag, long>
    {
        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;

        public CreateUserTagHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }

        public async Task<long> Handle(CreateUserTag request, CancellationToken cancellationToken)
        {
            var req = await _emailTopicsQueryRepository.CreateUserTagAsync(request);
            return req;
        }
    }

    public class CreateTimelineEventHandler : IRequestHandler<CreateEmailTimelineEvent, long>
    {
        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;

        public CreateTimelineEventHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }

        public async Task<long> Handle(CreateEmailTimelineEvent request, CancellationToken cancellationToken)
        {
            var req = await _emailTopicsQueryRepository.CreateEmailTimelineEventAsync(request);
            return req;
        }
    }
    public class UpdateTimelineEventHandler : IRequestHandler<UpdateEmailTimelineEvent, long>
    {
        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;

        public UpdateTimelineEventHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }

        public async Task<long> Handle(UpdateEmailTimelineEvent request, CancellationToken cancellationToken)
        {
            var req = await _emailTopicsQueryRepository.UpdateEmailTimelineEventAsync(request);
            return req;
        }
    }
    public class GetTimelineEventHandler : IRequestHandler<GetTimelineEventList, List<EmailTimelineEvent>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetTimelineEventHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTimelineEvent>> Handle(GetTimelineEventList request, CancellationToken cancellationToken)
        {
            return (List<EmailTimelineEvent>)await _emailTopicsQueryRepository.GetEmailTimelineEvent(request.DocumentId);
        }
    }
    public class CreateEmailDocFileProfileTypeHandler : IRequestHandler<CreateEmailDocFileProfileType, long>
    {
        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        private readonly IDocumentsQueryRepository _documentsQueryRepository;

        public CreateEmailDocFileProfileTypeHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository, IDocumentsQueryRepository documentsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
            _documentsQueryRepository = documentsQueryRepository;
        }

        public async Task<long> Handle(CreateEmailDocFileProfileType request, CancellationToken cancellationToken)
        {
            var result = await _documentsQueryRepository.GetByDocIdAsync(request.DocumentId);

            var docResult = new Documents()
            {
                FilterProfileTypeId = request.FilterProfileTypeId,
                FileName = result.FileName,
                ContentType = result.ContentType,
                FileSize = result.FileSize,
                UploadDate = request.UploadDate,
                SessionId = request.SessionId,
                AddedByUserId = request.AddedByUserId,
                AddedDate = request.AddedDate,
                IsLatest = true,
                FilePath = result.FilePath,
                IsNewPath = true,
                SourceFrom = "FileProfile"
            };

            var req = await _documentsQueryRepository.InsertCreateDocumentBySession(docResult);            
            return req.DocumentId;
        }
    }

    public class UpdateEmailDmsDocIdHandler : IRequestHandler<UpdateEmailDmsDocId, long>
    {        
        private readonly IDocumentsQueryRepository _documentsQueryRepository;

        public UpdateEmailDmsDocIdHandler(IDocumentsQueryRepository documentsQueryRepository)
        {            
            _documentsQueryRepository = documentsQueryRepository;
        }

        public async Task<long> Handle(UpdateEmailDmsDocId request, CancellationToken cancellationToken)
        {     
            var req = await _documentsQueryRepository.UpdateEmailDMS(request.DocumentId,request.EmailToDMS);
            return req;
        }
    }

    public class UpdateEmailTopicSubjectDueDateHandler : IRequestHandler<UpdateEmailTopicSubjectDueDate, long>
    {
        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;

        public UpdateEmailTopicSubjectDueDateHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;

        }

        public async Task<long> Handle(UpdateEmailTopicSubjectDueDate request, CancellationToken cancellationToken)
        {
            var req = await _emailTopicsQueryRepository.UpdateSubjectDueDate(request);
            return req;
        }
    }
    public class UpdateEmailSubjectNameHandler : IRequestHandler<UpdateEmailSubjectName, long>
    {
        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;

        public UpdateEmailSubjectNameHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;

        }

        public async Task<long> Handle(UpdateEmailSubjectName request, CancellationToken cancellationToken)
        {
            var req = await _emailTopicsQueryRepository.UpdateSubjectName(request);
            return req;
        }
    }
    
    public class UpdateEmailTopicClosedHandler : IRequestHandler<UpdateEmailTopicClosed, long>
    {
        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;

        public UpdateEmailTopicClosedHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;

        }

        public async Task<long> Handle(UpdateEmailTopicClosed request, CancellationToken cancellationToken)
        {
            var req = await _emailTopicsQueryRepository.UpdateTopicClose(request);
            return req;
        }
    }

    public class CreateEmailParticipantHandler : IRequestHandler<CreateEmailTopicParticipant, long>
    {
        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;

        public CreateEmailParticipantHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<long> Handle(CreateEmailTopicParticipant request, CancellationToken cancellationToken)
        {
            //var newTopics = await _emailTopicsQueryRepository.InsertParticipant(request);            
            //return newTopics;

            var newTopics = _emailTopicsQueryRepository.Insert_sp_Participant(request);
            var customerResponse = RoleMapper.Mapper.Map<long>(newTopics);
            return customerResponse;
        }
    }
    public class GetAllEmailTopicsHandler : IRequestHandler<GetAllEmailTopics, List<EmailTopics>>
    {

        private readonly IQueryRepository<EmailTopics> _queryRepository;
        public GetAllEmailTopicsHandler(IQueryRepository<EmailTopics> queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetAllEmailTopics request, CancellationToken cancellationToken)
        {
            return (List<EmailTopics>)await _queryRepository.GetListAsync();            
        }
    }
    public class GetActivityEmailTopicsHandler : IRequestHandler<GetActivityEmailTopics, List<ActivityEmailTopics>>
    {

        private readonly IQueryRepository<ActivityEmailTopics> _queryRepository;
        public GetActivityEmailTopicsHandler(IQueryRepository<ActivityEmailTopics> queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ActivityEmailTopics>> Handle(GetActivityEmailTopics request, CancellationToken cancellationToken)
        {
            return (List<ActivityEmailTopics>)await _queryRepository.GetListAsync();
        }
    }
    public class GetByActivityEmailSessionHandler : IRequestHandler<GetByActivityEmailSession, List<ActivityEmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetByActivityEmailSessionHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<ActivityEmailTopics>> Handle(GetByActivityEmailSession request, CancellationToken cancellationToken)
        {
            return (List<ActivityEmailTopics>)await _emailTopicsQueryRepository.GetByActivityEmailSessionList(request.EmailTopicSessionId);
        }
    }
    public class GetActivityEmailListHandler : IRequestHandler<GetActivityEmailList, List<ActivityEmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetActivityEmailListHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<ActivityEmailTopics>> Handle(GetActivityEmailList request, CancellationToken cancellationToken)
        {
            return (List<ActivityEmailTopics>)await _emailTopicsQueryRepository.GetActivityEmailListBySession(request.SessionId);
        }
    }
    public class GetActivityEmailDocListHandler : IRequestHandler<GetActivityEmailDocList, List<ActivityEmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetActivityEmailDocListHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<ActivityEmailTopics>> Handle(GetActivityEmailDocList request, CancellationToken cancellationToken)
        {
            return (List<ActivityEmailTopics>)await _emailTopicsQueryRepository.GetActivityEmailDocListBySession(request.SessionId);
        }
    }
    
    public class GetCreateEmailDocumentsHandler : IRequestHandler<GetAllCreateEmailDocumentLst, List<Documents>>
    {
        private readonly IEmailTopicsQueryRepository _createEmailDocumentsQueryRepository;

        public GetCreateEmailDocumentsHandler(IEmailTopicsQueryRepository createEmailDocumentsQueryRepository)
        {

            _createEmailDocumentsQueryRepository = createEmailDocumentsQueryRepository;
        }
        public async Task<List<Documents>> Handle(GetAllCreateEmailDocumentLst request, CancellationToken cancellationToken)
        {
            return (List<Documents>)await _createEmailDocumentsQueryRepository.GetCreateEmailDocumentListAsync(request.SessionId);
        }
    }
    public class GetDynamicFormDocHandler : IRequestHandler<GetDynamicFormDoc, List<Documents>>
    {
        private readonly IEmailTopicsQueryRepository _createEmailDocumentsQueryRepository;

        public GetDynamicFormDocHandler(IEmailTopicsQueryRepository createEmailDocumentsQueryRepository)
        {

            _createEmailDocumentsQueryRepository = createEmailDocumentsQueryRepository;
        }
        public async Task<List<Documents>> Handle(GetDynamicFormDoc request, CancellationToken cancellationToken)
        {
            return (List<Documents>)await _createEmailDocumentsQueryRepository.GetDynamicFormDocumentListAsync(request.SessionId);
        }
    }
    public class GetDynamicFormNameHandler : IRequestHandler<GetDynamicFormName, List<DynamicFormData>>
    {
        private readonly IEmailTopicsQueryRepository _createEmailDocumentsQueryRepository;

        public GetDynamicFormNameHandler(IEmailTopicsQueryRepository createEmailDocumentsQueryRepository)
        {

            _createEmailDocumentsQueryRepository = createEmailDocumentsQueryRepository;
        }
        public async Task<List<DynamicFormData>> Handle(GetDynamicFormName request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormData>)await _createEmailDocumentsQueryRepository.GetDynamicFormNameAsync(request.SessionId);
        }
    }
    


    public class GetPATypeDocLstHandler : IRequestHandler<GetPATypeDocLst, List<Documents>>
    {
        private readonly IEmailTopicsQueryRepository _createEmailDocumentsQueryRepository;

        public GetPATypeDocLstHandler(IEmailTopicsQueryRepository createEmailDocumentsQueryRepository)
        {

            _createEmailDocumentsQueryRepository = createEmailDocumentsQueryRepository;
        }
        public async Task<List<Documents>> Handle(GetPATypeDocLst request, CancellationToken cancellationToken)
        {
            return (List<Documents>)await _createEmailDocumentsQueryRepository.GetPATypeDocLstAsync(request.Id,request.Type);
        }
    }
    

    public class CreateActivityEmailHandler : IRequestHandler<CreateActivityEmail,long>
    {
        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;

        public CreateActivityEmailHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {

            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<long> Handle(CreateActivityEmail request, CancellationToken cancellationToken)
        {
            return (long)await _emailTopicsQueryRepository.CreateActivityEmailAsync(request);
        }
    }

    public class UpdateActivityEmailHandler : IRequestHandler<UpdateActivityEmail, long>
    {
        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;

        public UpdateActivityEmailHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {

            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<long> Handle(UpdateActivityEmail request, CancellationToken cancellationToken)
        {
            return (long)await _emailTopicsQueryRepository.UpdateActivityEmailAsync(request);
        }
    }

    public class DeleteDraftFileHandler : IRequestHandler <DeleteDocumentFileQuery , long>
    {
        private readonly IEmailTopicsQueryRepository _draftFileListQueryRepository;

        public DeleteDraftFileHandler(IEmailTopicsQueryRepository draftFileListQueryRepository, IQueryRepository<Documents> queryRepository)
        {
            _draftFileListQueryRepository = draftFileListQueryRepository;
        }

        public async Task<long> Handle(DeleteDocumentFileQuery request, CancellationToken cancellationToken)
        {
            var req = await _draftFileListQueryRepository.Delete(request.ID);
            return req;


        }
    }
}
