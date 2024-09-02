using Application.Queries.Base;
using Application.Response;
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
    public class GetAllEmailActivityCatgorys : PagedRequest, IRequest<List<EmailActivityCatgorys>>
    {
    }
    public class GetAllTopicCategory : PagedRequest, IRequest<List<EmailActivityCatgorys>>
    {
        public long TopicId { get; set; }
        public GetAllTopicCategory(long TopicId)
        {
            this.TopicId = TopicId;
        }
    }

    public class CreateEmailActivityCatgorysQuery : EmailActivityCatgorys, IRequest<long>
    {
    }
    public class EditTopicCategoryQery : EmailActivityCatgorys, IRequest<long>
    {
    }
    public class EditOtherTagQery : PagedRequest, IRequest<string>
    {
        public string otherTag { get; set; }
        public string Name { get; set; }
        public EditOtherTagQery(string otherTag, string Name)
        {
            this.otherTag = otherTag;
            this.Name = Name;
        }
    }
    public class EditUserTagQery : PagedRequest, IRequest<string>
    {
        public string userTag { get; set; }
        public string Name { get; set; }
        public EditUserTagQery(string userTag, string name)
        {
            this.userTag = userTag;
            this.Name = name;
        }
    }
    public class DeleteTopicCategoryQery : PagedRequest, IRequest<long>
    {
        public long ID { get; set; }
        public DeleteTopicCategoryQery(long ID)
        {
            this.ID = ID;
        }
    }
    public class GetAllemailCategory : PagedRequest, IRequest<List<EmailActivityCatgorys>>
    {
        public long TopicId { get; set; }
        public GetAllemailCategory(long TopicId)
        {
            this.TopicId = TopicId;
        }
    }

    public class GetAllUserActivityCatgorys : PagedRequest, IRequest<List<EmailActivityCatgorys>>
    {
        public long UserID { get; set; }
        public GetAllUserActivityCatgorys(long UserID)
        {
            this.UserID = UserID;
        }
    }
    public class GetByUserTage : PagedRequest, IRequest<List<EmailActivityCatgorys>>
    {
        public long TopicID { get; set; }
        public long UserID { get; set; }
        public GetByUserTage(long topicId,long UserID)
        {
            this.TopicID = topicId;
            this.UserID = UserID;
        }
    }
}
