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
    public class GetAllForumConversation : PagedRequest, IRequest<List<ForumConversations>>
    {
        public string SearchString { get; set; }
    }
    public class CreateForumCoversation : ForumConversations, IRequest<long>
    {
        public string SearchString { get; set; }
    }
    public class EditForumConversation : ForumConversations, IRequest<long>
    {
        public string SearchString { get; set; }
    }
    public class DeleteForumConversation : ForumConversations, IRequest<long>
    {
        public string SearchString { get; set; }
    }
    public class DeleteParticipant : TopicParticipant, IRequest<long>
    {        
    }
    public class GetAllParticipantListQuery : PagedRequest, IRequest<List<ViewEmployee>>
    {
        public long TopicId { get; private set; }
        public GetAllParticipantListQuery(long topicId)
        {
            this.TopicId = topicId;
        }
    }
    public class GetDiscussionList : PagedRequest, IRequest<List<ForumConversations>>
    {
        public long TopicId { get; private set; }
        public GetDiscussionList(long TopicId)
        {
            this.TopicId = TopicId;
        }
    }
    
}
