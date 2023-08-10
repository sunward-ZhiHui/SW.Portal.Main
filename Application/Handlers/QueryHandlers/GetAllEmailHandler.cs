using Application.Common.Mapper;
using Application.Queries;
using Application.Response;
using Core.Entities;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;

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
    public class GetEmailTopicToHandler : IRequestHandler<GetEmailTopicTo, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetEmailTopicToHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetEmailTopicTo request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetTopicToList(request.UserId);

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
            return await _emailTopicsQueryRepository.GetTopicAllList(request.UserId);

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
            return await _emailTopicsQueryRepository.GetTopicMasterSearchList(request.UserId,request.MasterSearch);

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
            return await _emailTopicsQueryRepository.GetTopicCCList(request.UserId);

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
            return await _emailTopicsQueryRepository.GetSubTopicToList(request.TopicId,request.UserId);

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
            return await _emailTopicsQueryRepository.GetSubTopicAllList(request.TopicId, request.UserId);

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
            return await _emailTopicsQueryRepository.GetSubTopicCCList(request.TopicId, request.UserId);

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
            return await _emailTopicsQueryRepository.GetSubTopicSentList(request.TopicId, request.UserId);

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
            return await _emailTopicsQueryRepository.GetTopicSentList(request.UserId);

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
