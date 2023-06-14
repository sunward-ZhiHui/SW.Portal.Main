using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllEmailConversation : PagedRequest, IRequest<List<EmailConversations>>
    {
        public string SearchString { get; set; }
    }
    public class CreateEmailCoversation : EmailConversations, IRequest<long>
    {
        public string SearchString { get; set; }
    }
    public class EditEmailConversation : EmailConversations, IRequest<long>
    {
        public string SearchString { get; set; }
    }
    public class DeleteEmailConversation : EmailConversations, IRequest<long>
    {
        public string SearchString { get; set; }
    }
    public class DeleteEmailParticipant : TopicParticipant, IRequest<long>
    {        
    }
    public class GetAllEmailParticipantListQuery : PagedRequest, IRequest<List<ViewEmployee>>
    {
        public long TopicId { get; private set; }
        public GetAllEmailParticipantListQuery(long topicId)
        {
            this.TopicId = topicId;
        }
    }
    public class GetEmailDiscussionList : PagedRequest, IRequest<List<EmailConversations>>
    {
        public long TopicId { get; private set; }
        public GetEmailDiscussionList(long TopicId)
        {
            this.TopicId = TopicId;
        }
    }
    public class GetEmailTopicDocList : PagedRequest, IRequest<List<Documents>>
    {
        public long TopicId { get; private set; }
        public GetEmailTopicDocList(long TopicId)
        {
            this.TopicId = TopicId;
        }
    }
    public class GetEmailAssignToList : PagedRequest, IRequest<List<EmailAssignToList>>
    {
        public long TopicId { get; set; }
        public GetEmailAssignToList(long topicId)
        {
            this.TopicId = topicId;
        }
    }
    public class GetEmailTopicToList : PagedRequest, IRequest<List<EmailTopicTo>>
    {
        public long TopicId { get; set; }
        public GetEmailTopicToList(long topicId)
        {
            this.TopicId = topicId;
        }
    }
    public class GetEmailConversationAssignTo : PagedRequest, IRequest<List<EmailConversationAssignTo>>
    {
        public long ConversationId { get; set; }
        public GetEmailConversationAssignTo(long Id)
        {
            this.ConversationId = Id;
        }
    }

}
