using Application.Queries;
using Core.Entities;
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
    public class GetAllDynamicFormHandler : IRequestHandler<GetAllDynamicForm, List<DynamicForm>>
    {
        private readonly IDynamicFormQueryRepository _topicTodoListQueryRepository;
        public GetAllDynamicFormHandler(IDynamicFormQueryRepository topicTodoListQueryRepository)
        {
            _topicTodoListQueryRepository = topicTodoListQueryRepository;
        }
        public async Task<List<DynamicForm>> Handle(GetAllDynamicForm request, CancellationToken cancellationToken)
        {
            return (List<DynamicForm>)await _topicTodoListQueryRepository.GetAllAsync();
        }

        
    }

    public class CreateDynamicFormHandler : IRequestHandler<CreateDynamicForm, long>
    {

      
        private readonly IDynamicFormQueryRepository _queryRepository;
        public CreateDynamicFormHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
          
        }
        public async Task<long> Handle(CreateDynamicForm request, CancellationToken cancellationToken)
        {
            return (long)await _queryRepository.Insert(request);

        }
    }
}
