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
    }
    public class CreateTopicTodoListQuery : TopicToDoList, IRequest<long>
    {
    }
    public class EditTopicTodoListQuery : TopicToDoList, IRequest<long>
    {
    }
    public class DeleteTopicToDoListQuery : TopicToDoList, IRequest<long>
    {
        public Int64 Id { get; private set; }

        public DeleteTopicToDoListQuery(Int64 Id)
        {
            this.Id = Id;
        }
    }
}
