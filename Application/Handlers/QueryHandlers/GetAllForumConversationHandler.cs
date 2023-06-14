using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Command.Base;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using Microsoft.AspNetCore.Http;
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

            var listData = request.AssigntoIds.ToList();
            if (listData.Count > 0)
            {
                request.AssigntoIds.ToList().ForEach(a =>
                {
                    var conversationAssignTo = new ForumConversationAssignTo();
                    conversationAssignTo.ConversationId = req;
                    conversationAssignTo.TopicId = request.TopicID;
                    conversationAssignTo.UserId = a;
                    conversationAssignTo.StatusCodeID = request.StatusCodeID;
                    conversationAssignTo.AddedByUserID = request.AddedByUserID;
                    conversationAssignTo.SessionId = request.SessionId;
                    conversationAssignTo.AddedDate = request.AddedDate;
                    _conversationQueryRepository.InsertAssignTo(conversationAssignTo);
                });
            }

            var plistData = request.AllParticipantIds.ToList();
            if (plistData.Count > 0)
            {
                request.AllParticipantIds.ToList().ForEach(a =>
                {
                    var forumNotifications = new ForumNotifications();
                    forumNotifications.ConversationId = req;
                    forumNotifications.TopicId = request.TopicID;
                    forumNotifications.UserId = a;
                    forumNotifications.AddedByUserID = request.AddedByUserID;
                    forumNotifications.AddedDate = request.AddedDate;
                    forumNotifications.IsRead = true;
                    _conversationQueryRepository.InsertForumNotifications(forumNotifications);
                });
            }


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
    public class GetAssignToListHandler : IRequestHandler<GetAssignToList, List<ForumAssignToList>>
    {
        private readonly IForumConversationsQueryRepository _conversationQueryRepository;
        public GetAssignToListHandler(IForumConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<ForumAssignToList>> Handle(GetAssignToList request, CancellationToken cancellationToken)
        {
            return (List<ForumAssignToList>)await _conversationQueryRepository.GetAllAssignToListAsync(request.TopicId);
        }
    }
    public class GetTopicToListHandler : IRequestHandler<GetTopicToList, List<ForumTopicTo>>
    {
        private readonly IForumConversationsQueryRepository _conversationQueryRepository;
        public GetTopicToListHandler(IForumConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<ForumTopicTo>> Handle(GetTopicToList request, CancellationToken cancellationToken)
        {
            return (List<ForumTopicTo>)await _conversationQueryRepository.GetTopicToListAsync(request.TopicId);
        }
    }
    public class GetConversationAssignToHandler : IRequestHandler<GetConversationAssignTo, List<ForumConversationAssignTo>>
    {
        private readonly IForumConversationsQueryRepository _conversationQueryRepository;
        public GetConversationAssignToHandler(IForumConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<ForumConversationAssignTo>> Handle(GetConversationAssignTo request, CancellationToken cancellationToken)
        {
            return (List<ForumConversationAssignTo>)await _conversationQueryRepository.GetConversationAssignToList(request.ConversationId);
        }
    }
}