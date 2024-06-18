using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
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
    public class GetAllDynamicFormListQueryHandler : IRequestHandler<GetAllDynamicFormItemListQuery, List<DynamicFormItem>>
    {
        private readonly IDynamicFormItemQueryRepository _queryRepository;
        public GetAllDynamicFormListQueryHandler(IDynamicFormItemQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<DynamicFormItem>> Handle(GetAllDynamicFormItemListQuery request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormItem>)await _queryRepository.GetAllAsync();
        }
    }
    public class CreateDynamicFormItemListHandler : IRequestHandler<CreateDynamicFormItemList, long>
    {
        private readonly IDynamicFormItemQueryRepository _QueryRepository;
        public CreateDynamicFormItemListHandler(IDynamicFormItemQueryRepository QueryRepository)
        {
            _QueryRepository = QueryRepository;
        }

        public async Task<long> Handle(CreateDynamicFormItemList request, CancellationToken cancellationToken)
        {
            var newlist = await _QueryRepository.Insert(request);
            return newlist;

        }

    }
    public class EditDynamicFormItemListListHandler : IRequestHandler<EditDynamicFormItemList, long>
    {
        private readonly IDynamicFormItemQueryRepository _QueryRepository;
        public EditDynamicFormItemListListHandler(IDynamicFormItemQueryRepository QueryRepository)
        {
            _QueryRepository = QueryRepository;
        }

        public async Task<long> Handle(EditDynamicFormItemList request, CancellationToken cancellationToken)
        {
            var req = await _QueryRepository.Update(request);
            return req;
        }
    }
    public class DeleteDynamicFormItemListListHandler : IRequestHandler<DeleteDynamicFormItemListt, long>
    {
        private readonly IDynamicFormItemQueryRepository _QueryRepository;

        public DeleteDynamicFormItemListListHandler(IDynamicFormItemQueryRepository QueryRepository)
        {
            _QueryRepository = QueryRepository;
        }

        public async Task<long> Handle(DeleteDynamicFormItemListt request, CancellationToken cancellationToken)
        {
            var req = await _QueryRepository.Delete(request.ID);
            return req;
        }
    }


    public class EditDynamicFormItemLineQueryHandler : IRequestHandler<EditDynamicFormItemLine, long>
    {
        private readonly IDynamicFormItemQueryRepository _QueryRepository;
        public EditDynamicFormItemLineQueryHandler(IDynamicFormItemQueryRepository QueryRepository)
        {
            _QueryRepository = QueryRepository;
        }

        public async Task<long> Handle(EditDynamicFormItemLine request, CancellationToken cancellationToken)
        {
            var req = await _QueryRepository.Update(request);
            return req;
        }
    }
    public class CreateDynamicFormItemLineQueryHandler : IRequestHandler<CreateDynamicFormItemLine, long>
    {
        private readonly IDynamicFormItemQueryRepository _QueryRepository;
        public CreateDynamicFormItemLineQueryHandler(IDynamicFormItemQueryRepository QueryRepository)
        {
            _QueryRepository = QueryRepository;
        }

        public async Task<long> Handle(CreateDynamicFormItemLine request, CancellationToken cancellationToken)
        {
            var newlist = await _QueryRepository.Insert(request);
            return newlist;

        }

    }

    public class DeleteDynamicFormItemLineQueryHandler : IRequestHandler<DeleteDynamicFormItemLine, long>
    {
        private readonly IDynamicFormItemQueryRepository _topicTodoListQueryRepository;

        public DeleteDynamicFormItemLineQueryHandler(IDynamicFormItemQueryRepository topicTodoListQueryRepository)
        {
            _topicTodoListQueryRepository = topicTodoListQueryRepository;
        }

        public async Task<long> Handle(DeleteDynamicFormItemLine request, CancellationToken cancellationToken)
        {
            var req = await _topicTodoListQueryRepository.DeleteLine(request.DynamicFormItemLineID);
            return req;
        }
    }
    public class GetAllDynamicItemFormLineQueryHandler : IRequestHandler<GetAllDynamicItemFormLineList, List<DynamicFormItemLine>>
    {
        private readonly IDynamicFormItemQueryRepository _QueryRepository;
        public GetAllDynamicItemFormLineQueryHandler(IDynamicFormItemQueryRepository QueryRepository)
        {
            _QueryRepository = QueryRepository;
        }
        public async Task<List<DynamicFormItemLine>> Handle(GetAllDynamicItemFormLineList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormItemLine>)await _QueryRepository.GetAllDynamicLineAsync(request.DynamicFormItemID);
           
          
        }

    }


    public class GetAllDynamicFormDropdownQueryHandler : IRequestHandler<GetAllDynamicFormDropdownListQuery, List<DynamicForm>>
    {
        private readonly IDynamicFormItemQueryRepository _queryRepository;
        public GetAllDynamicFormDropdownQueryHandler(IDynamicFormItemQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<DynamicForm>> Handle(GetAllDynamicFormDropdownListQuery request, CancellationToken cancellationToken)
        {
            return (List<DynamicForm>)await _queryRepository.GetAllDynamicFormDropdownAsync();
        }
    }

    public class GetDynamicFormItemMasterHandler : IRequestHandler<GetDynamicFormItemMasterList, List<DynamicFormItem>>
    {
        private readonly IDynamicFormItemQueryRepository _QueryRepository;

        public GetDynamicFormItemMasterHandler(IDynamicFormItemQueryRepository QueryRepository)
        {
            _QueryRepository = QueryRepository;
        }

        public async Task<List<DynamicFormItem>> Handle(GetDynamicFormItemMasterList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormItem>)await _QueryRepository.GetAllDynamicAsync(request.DynamicFormItemID);
        }
    }
    public class GetDynamicFormItemLineHandler : IRequestHandler<GetDynamicFormItemLineDropdoenList, DropDownOptionsGridListModel>
    {
        private readonly IAttributeQueryRepository _QueryRepository;

        public GetDynamicFormItemLineHandler(IAttributeQueryRepository QueryRepository)
        {
            _QueryRepository = QueryRepository;
        }

        public async Task<DropDownOptionsGridListModel> Handle(GetDynamicFormItemLineDropdoenList request, CancellationToken cancellationToken)
        {
           var dynamiclist =await _QueryRepository.GetDynamicGridDropDownById(request.DynamicFormId,request.Userid);
            return dynamiclist;
        }
    }
}
