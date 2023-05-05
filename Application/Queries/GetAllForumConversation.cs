using Application.Queries.Base;
using Core.Entities;
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
}
