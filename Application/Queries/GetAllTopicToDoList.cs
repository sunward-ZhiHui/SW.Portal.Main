using Application.Queries.Base;
using Application.Response;
using Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllTopicToDoList : PagedRequest, IRequest<List<TopicToDoList>>
    {
        public long Uid { get; set; }
        public GetAllTopicToDoList(long id)
        {
            this.Uid = id;
        }
    }
    public class CreateTopicTodoListQuery : TopicToDoList, IRequest<long>
    {
    }
    public class EditTopicTodoListQuery : TopicToDoList, IRequest<long>
    {
    }
    public class DeleteTopicToDoListQuery : TopicToDoList, IRequest<long>
    {
        public long ID { get;set; }

        public DeleteTopicToDoListQuery(long Id)
        {
            this.ID = Id;
        }
    }
}
