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
    public class GetAllEmailConversationHandler : IRequestHandler<GetAllEmailConversation, List<EmailConversations>>
    {

        private readonly IQueryRepository<EmailConversations> _queryRepository;
        public GetAllEmailConversationHandler(IQueryRepository<EmailConversations> queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<EmailConversations>> Handle(GetAllEmailConversation request, CancellationToken cancellationToken)
        {
            return (List<EmailConversations>)await _queryRepository.GetListAsync();
            //return (List<EmailTypes>)await _roleQueryRepository.GetAllAsync();
        }
    }
    //Get discussion list
    public class GetEmailDiscussionListHandler : IRequestHandler<GetEmailDiscussionList, List<EmailConversations>>
    {
        private readonly IEmailConversationsQueryRepository _emailConversationsQueryRepository;
        
        public GetEmailDiscussionListHandler(IEmailConversationsQueryRepository emailConversationsQueryRepository)
        {

            _emailConversationsQueryRepository = emailConversationsQueryRepository;
        }
        public async Task<List<EmailConversations>> Handle(GetEmailDiscussionList request, CancellationToken cancellationToken)
        {
            return (List<EmailConversations>)await _emailConversationsQueryRepository.GetDiscussionListAsync(request.TopicId);           
        }
    }
    public class GetEmailFullDiscussionListHandler : IRequestHandler<GetEmailFullDiscussionList, List<EmailConversations>>
    {
        private readonly IEmailConversationsQueryRepository _emailConversationsQueryRepository;

        public GetEmailFullDiscussionListHandler(IEmailConversationsQueryRepository emailConversationsQueryRepository)
        {

            _emailConversationsQueryRepository = emailConversationsQueryRepository;
        }
        public async Task<List<EmailConversations>> Handle(GetEmailFullDiscussionList request, CancellationToken cancellationToken)
        {
            return (List<EmailConversations>)await _emailConversationsQueryRepository.GetFullDiscussionListAsync(request.TopicId);
        }
    }
    
    public class GetEmailTopicDocListHandler : IRequestHandler<GetEmailTopicDocList, List<Documents>>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;

        public GetEmailTopicDocListHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {

            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<Documents>> Handle(GetEmailTopicDocList request, CancellationToken cancellationToken)
        {
            return (List<Documents>)await _conversationQueryRepository.GetTopicDocListAsync(request.TopicId);
        }
    }


    public class CreateEmailConversationHandler : IRequestHandler<CreateEmailCoversation, long>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;

        public CreateEmailConversationHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;

        }

        public async Task<long> Handle(CreateEmailCoversation request, CancellationToken cancellationToken)
        {
            var req = await _conversationQueryRepository.Insert(request);

            var listData = request.AssigntoIds.ToList();
            if (listData.Count > 0)
            {
                request.AssigntoIds.ToList().ForEach(a =>
                {
                    var conversationAssignTo = new EmailConversationAssignTo();
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
                    var forumNotifications = new EmailNotifications();
                    forumNotifications.ConversationId = req;
                    forumNotifications.TopicId = request.TopicID;
                    forumNotifications.UserId = a;
                    forumNotifications.AddedByUserID = request.AddedByUserID;
                    forumNotifications.AddedDate = request.AddedDate;
                    forumNotifications.IsRead = true;
                    _conversationQueryRepository.InsertEmailNotifications(forumNotifications);
                });
            }


            return req;
        }
    }
    public class EditEmailConversationHandler : IRequestHandler<EditEmailConversation, long>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;

        public EditEmailConversationHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;

        }

        public async Task<long> Handle(EditEmailConversation request, CancellationToken cancellationToken)
        {
            var req = await _conversationQueryRepository.Update(request);
            return req;
        }
    }
    public class DeleteEmailConversationHandler : IRequestHandler<DeleteEmailConversation, long>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;

        public DeleteEmailConversationHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;

        }

        public async Task<long> Handle(DeleteEmailConversation request, CancellationToken cancellationToken)
        {
            var req = await _conversationQueryRepository.Delete(request);
            return req;
        }
    }

    public class DeleteEmailParticipantHandler : IRequestHandler<DeleteEmailParticipant, long>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;

        public DeleteEmailParticipantHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;

        }

        public async Task<long> Handle(DeleteEmailParticipant request, CancellationToken cancellationToken)
        {
            var req = await _conversationQueryRepository.DeleteParticipant(request);
            return req;
        }
    }

    public class GetAllEmailParticipantListHandler : IRequestHandler<GetAllEmailParticipantListQuery, List<ViewEmployee>>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;
        public GetAllEmailParticipantListHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<ViewEmployee>> Handle(GetAllEmailParticipantListQuery request, CancellationToken cancellationToken)
        {
            return (List<ViewEmployee>)await _conversationQueryRepository.GetAllParticipantAsync(request.TopicId);
        }
    }
    public class GetEmailAssignToListHandler : IRequestHandler<GetEmailAssignToList, List<EmailAssignToList>>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;
        public GetEmailAssignToListHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<EmailAssignToList>> Handle(GetEmailAssignToList request, CancellationToken cancellationToken)
        {
            return (List<EmailAssignToList>)await _conversationQueryRepository.GetAllAssignToListAsync(request.TopicId);
        }
    }
    public class GetEmailTopicToListHandler : IRequestHandler<GetEmailTopicToList, List<EmailTopicTo>>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;
        public GetEmailTopicToListHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<EmailTopicTo>> Handle(GetEmailTopicToList request, CancellationToken cancellationToken)
        {
            return (List<EmailTopicTo>)await _conversationQueryRepository.GetTopicToListAsync(request.TopicId);
        }
    }
    public class GetEmailConversationAssignToHandler : IRequestHandler<GetEmailConversationAssignTo, List<EmailConversationAssignTo>>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;
        public GetEmailConversationAssignToHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<EmailConversationAssignTo>> Handle(GetEmailConversationAssignTo request, CancellationToken cancellationToken)
        {
            return (List<EmailConversationAssignTo>)await _conversationQueryRepository.GetConversationAssignToList(request.ConversationId);
        }
    }
}