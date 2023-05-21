using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Command.Base;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllForumConversationHandler : IRequestHandler<GetAllForumConversation, List<ForumConversations>>
    {

        private readonly IQueryRepository<ForumConversations> _queryRepository;
        public GetAllForumConversationHandler(IQueryRepository<ForumConversations> queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ForumConversations>> Handle(GetAllForumConversation request, CancellationToken cancellationToken)
        {
            return (List<ForumConversations>)await _queryRepository.GetListAsync();
            //return (List<ForumTypes>)await _roleQueryRepository.GetAllAsync();
        }
    }
    //Get discussion list
    public class GetDiscussionListHandler : IRequestHandler<GetDiscussionList, List<ForumConversations>>
    {
        private readonly IForumConversationsQueryRepository _conversationQueryRepository;
        
        public GetDiscussionListHandler(IForumConversationsQueryRepository conversationQueryRepository)
        {
            
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<ForumConversations>> Handle(GetDiscussionList request, CancellationToken cancellationToken)
        {
            return (List<ForumConversations>)await _conversationQueryRepository.GetDiscussionListAsync(request.TopicId);           
        }
    }
    public class GetTopicDocListHandler : IRequestHandler<GetTopicDocList, List<Documents>>
    {
        private readonly IForumConversationsQueryRepository _conversationQueryRepository;

        public GetTopicDocListHandler(IForumConversationsQueryRepository conversationQueryRepository)
        {

            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<Documents>> Handle(GetTopicDocList request, CancellationToken cancellationToken)
        {
            return (List<Documents>)await _conversationQueryRepository.GetTopicDocListAsync(request.TopicId);
        }
    }


    public class CreateForumConversationHandler : IRequestHandler<CreateForumCoversation, long>
    {
        private readonly IForumConversationsQueryRepository _conversationQueryRepository;

        public CreateForumConversationHandler(IForumConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;

        }

        public async Task<long> Handle(CreateForumCoversation request, CancellationToken cancellationToken)
        {
            var req = await _conversationQueryRepository.Insert(request);
            return req;
        }
    }
    public class EditForumConversationHandler : IRequestHandler<EditForumConversation, long>
    {
        private readonly IForumConversationsQueryRepository _conversationQueryRepository;

        public EditForumConversationHandler(IForumConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;

        }

        public async Task<long> Handle(EditForumConversation request, CancellationToken cancellationToken)
        {
            var req = await _conversationQueryRepository.Update(request);
            return req;
        }
    }
    public class DeleteForumConversationHandler : IRequestHandler<DeleteForumConversation, long>
    {
        private readonly IForumConversationsQueryRepository _conversationQueryRepository;

        public DeleteForumConversationHandler(IForumConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;

        }

        public async Task<long> Handle(DeleteForumConversation request, CancellationToken cancellationToken)
        {
            var req = await _conversationQueryRepository.Delete(request);
            return req;
        }
    }

    public class DeleteParticipantHandler : IRequestHandler<DeleteParticipant, long>
    {
        private readonly IForumConversationsQueryRepository _conversationQueryRepository;

        public DeleteParticipantHandler(IForumConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;

        }

        public async Task<long> Handle(DeleteParticipant request, CancellationToken cancellationToken)
        {
            var req = await _conversationQueryRepository.DeleteParticipant(request);
            return req;
        }
    }

    public class GetAllParticipantListHandler : IRequestHandler<GetAllParticipantListQuery, List<ViewEmployee>>
    {
        private readonly IForumConversationsQueryRepository _conversationQueryRepository;
        public GetAllParticipantListHandler(IForumConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<ViewEmployee>> Handle(GetAllParticipantListQuery request, CancellationToken cancellationToken)
        {
            return (List<ViewEmployee>)await _conversationQueryRepository.GetAllParticipantAsync(request.TopicId);
        }
    }
}