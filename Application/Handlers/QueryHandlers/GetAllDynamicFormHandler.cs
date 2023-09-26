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

    public class GetAllDynamicFormLstHandler : IRequestHandler<GetAllDynamicFormLst, DynamicForm>
    {

        private readonly IQueryRepository<AttributeHeader> _queryRepository;
        private readonly IDynamicFormQueryRepository _DynamicFormQueryRepository;
        public GetAllDynamicFormLstHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {

            _DynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<DynamicForm> Handle(GetAllDynamicFormLst request, CancellationToken cancellationToken)
        {
            return await _DynamicFormQueryRepository.GetAllSelectedLst(request.ID);
        }
    }

    public class EditDynamicFormHandler : IRequestHandler<EditDynamicForm, long>
    {
        private readonly IDynamicFormQueryRepository _DynamicFormQueryRepository;
        public EditDynamicFormHandler(IDynamicFormQueryRepository QueryRepository)
        {
            _DynamicFormQueryRepository = QueryRepository;
        }

        public async Task<long> Handle(EditDynamicForm request, CancellationToken cancellationToken)
        {
            var req = await _DynamicFormQueryRepository.Update(request);
            return req;
        }
    }
    public class DeleteDynamicFormHandler : IRequestHandler<DeleteDynamicForm, long>
    {
        private readonly IDynamicFormQueryRepository _DynamicFormQueryRepository;

        public DeleteDynamicFormHandler(IDynamicFormQueryRepository QueryRepository)
        {
            _DynamicFormQueryRepository = QueryRepository;
        }

        public async Task<long> Handle(DeleteDynamicForm request, CancellationToken cancellationToken)
        {
            var req = await _DynamicFormQueryRepository.Delete(request.ID);
            return req;
        }
    }
}
