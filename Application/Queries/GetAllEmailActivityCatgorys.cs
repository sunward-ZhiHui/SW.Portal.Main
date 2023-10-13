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
}
